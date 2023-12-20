'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Imports SudokuPuzzle.SharedFunctions

Namespace Model

    Friend Class GameModel
        ' This is the Model in the MVC programming pattern.  The model simply
        ' contains the data to be used by the V or View.  In the traditional
        ' MVC programming pattern, the model does not contain any business
        ' logic.  However, in this case, I put the code to build all the notes 
        ' for each Cell here.  When you think about it, the notes are really a
        ' part of the data so it makes sense to stick the code here.  I could
        ' have put the note generation code in with the game generator or up 
        ' in the game controller, but since I do not save the notes when the
        ' game exits, I can't really put the code in either place because the
        ' notes would be lost when we leave the game and come back since we
        ' only save the answer and the masking state.  I could also save 
        ' the notes, but that would take up more space in the config file.
        ' In the end, it's easier to just recompute the notes.  That's why I 
        ' put the code to generate the notes here since it's just a one time 
        ' thing when the Model is loaded with a new game.

#Region " Variables "

        Private _uCells(,) As CellStateClass
        Private _lCells As List(Of CellStateClass)
        Private _lRegionCells(9) As List(Of CellStateClass)

#End Region

#Region " Properties "

#Region " Public Read/Write Properties "

        Friend Property EmptyCount As Int32

#End Region

#Region " Public Readonly Properties "

        Friend ReadOnly Property GameComplete As Boolean
            Get
                Return (EmptyCount = 0)
            End Get
        End Property

        Friend ReadOnly Property RegionCells(iIndex As Int32) As List(Of CellStateClass)
            Get
                If Common.IsValidIndex(iIndex) Then
                    Return _lRegionCells(iIndex)
                End If
                Return Nothing
            End Get
        End Property

        Friend ReadOnly Property Cells As List(Of CellStateClass)
            Get
                Return _lCells
            End Get
        End Property

#End Region

#End Region

#Region " Constructors "

        Friend Sub New(uCells(,) As CellStateClass)
            InitClass(uCells)                                       ' Upon class instantiation, save the game
        End Sub

#End Region

#Region " Public Methods "

        Friend Sub ComputeNote(iCol As Int32, iRow As Int32)
            If Common.IsValidIndex(iCol, iRow) Then                 ' Are indices valid?
                GenerateNote(_uCells(iCol, iRow))                   ' Yes, generate the notes for this one cell
            End If
        End Sub

        Friend Function Cell(iCol As Int32, iRow As Int32) As CellStateClass
            If Common.IsValidIndex(iCol, iRow) Then                 ' Are indices valid?
                Return _uCells(iCol, iRow)                          ' Yes, return cell
            End If
            Return Nothing                                          ' Invalid indices.  Just return nothing.
        End Function

        Friend Function Cell(uIndex As CellIndex) As CellStateClass
            If Common.IsValidIndex(uIndex) Then                     ' Is index valid?
                Return _uCells(uIndex.Col, uIndex.Row)              ' Yes, return cell
            End If
            Return Nothing                                          ' Invalid indices.  Just return nothing.
        End Function

#End Region

#Region " Private Methods "

        Private Sub InitClass(uCells(,) As CellStateClass)
            If uCells IsNot Nothing Then                            ' Make sure that incoming array is not empty
                _uCells = uCells                                    ' Save a pointer to the game array
                ConvertToList()                                     ' Convert 2D array to List
                InitRegionList()                                    ' Build the regions
                GenerateAllNotes()                                  ' Generate the preliminary notes
                CountEmpties()                                      ' Count number of empty cells
            End If
        End Sub

        Private Sub ConvertToList()
            _lCells = New List(Of CellStateClass)               ' Initialize list variable
            For iCol As Int32 = 1 To 9                          ' Loop through all the cells
                For iRow As Int32 = 1 To 9
                    _lCells.Add(_uCells(iCol, iRow))            ' Add it to the list
                Next
            Next
        End Sub

        Private Sub InitRegionList()
            AddCellsToRegion(1, 1, 3, 1, 3)                     ' Add the different regions and their corresponding cells
            AddCellsToRegion(2, 4, 6, 1, 3)
            AddCellsToRegion(3, 7, 9, 1, 3)
            AddCellsToRegion(4, 1, 3, 4, 6)
            AddCellsToRegion(5, 4, 6, 4, 6)
            AddCellsToRegion(6, 7, 9, 4, 6)
            AddCellsToRegion(7, 1, 3, 7, 9)
            AddCellsToRegion(8, 4, 6, 7, 9)
            AddCellsToRegion(9, 7, 9, 7, 9)
        End Sub

        Private Sub AddCellsToRegion(iRegion As Int32, iColStart As Int32, iColEnd As Int32, iRowStart As Int32, iRowEnd As Int32)
            _lRegionCells(iRegion) = New List(Of CellStateClass)    ' Create a new List
            For iCol As Int32 = iColStart To iColEnd                ' For each column in the region
                For iRow As Int32 = iRowStart To iRowEnd            ' For each row in the region
                    _lRegionCells(iRegion).Add(_uCells(iCol, iRow)) ' Add the corresponding cell to this region
                Next
            Next
        End Sub

        Private Sub GenerateAllNotes()
            For I As Int32 = 0 To _lCells.Count - 1                 ' Loop through all the cels
                GenerateNote(_lCells(I))                            ' Generate notes for each cell
            Next
        End Sub

        Private Sub GenerateNote(uCell As CellStateClass)
            ' We won't use any of the solutions to generate notes.  For example, in the
            ' follow scenario:
            '      +--------+--------+--------+
            '      |.. .. ..| 1 ..  1|.. .. ..|
            '      |.. .. ..|.. .. ..|.. .. ..|
            '      | 1  1  1|.. .. ..|.. .. ..|
            '      +--------+--------+--------+
            ' If "1" in the first region can only be in the 3rd row and in the second region
            ' it can only be in the first row, then in the 3rd region, "1" can only be in
            ' the 2nd row.  But we're not going to apply any of these types of strategies
            ' The only strategy we will employ is if "1" is displayed in a row, column, 
            ' or region, then it will be removed from the note.
            If uCell.CellState <> CellStateEnum.Answer Then                     ' If we're not displaying the answer
                For I As Int32 = 1 To 9                                         ' Turn on all the notes by default
                    uCell.Notes(I) = True
                Next
                For iScan As Int32 = 1 To 9
                    ProcessNote(uCell, _uCells(uCell.Col, iScan))               ' Scan column and turn off notes that already exist
                    ProcessNote(uCell, _uCells(iScan, uCell.Row))               ' Scan row and turn off notes that already exist
                Next
                For Each Item As CellStateClass In _lRegionCells(uCell.Region)  ' Scan Region and turn off values that already exist
                    ProcessNote(uCell, Item)
                Next
            End If
        End Sub

        Private Sub ProcessNote(uTargetCell As CellStateClass, uSourceCell As CellStateClass)
            With uSourceCell
                If .CellState = CellStateEnum.Answer Then                   ' Are we displaying the answer?
                    uTargetCell.Notes(.Answer) = False                      ' Yes, then turn off the corresponding note.
                End If
            End With
        End Sub

        Private Sub CountEmpties()
            EmptyCount = 0                                                                  ' Zero counter
            For Each Item As CellStateClass In _lCells                                      ' Go through all the cells
                If (Item IsNot Nothing) AndAlso (Item.CellState = CellStateEnum.Blank) Then ' If item not null and its state is Blank
                    EmptyCount += 1                                                         ' Count it
                End If
            Next
        End Sub

#End Region

    End Class

End Namespace
