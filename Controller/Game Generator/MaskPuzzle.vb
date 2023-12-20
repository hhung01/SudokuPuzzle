'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Imports SudokuPuzzle.Model
Imports SudokuPuzzle.SharedFunctions

Namespace Controller.GameGenerator

    Friend Class MaskPuzzle
        ' After playing many Sudoku puzzles over the years, some of the more 
        ' difficult puzzles can have multiple solutions.  I find that these kinds of
        ' puzzles are not true Sudoku puzzles because at some point, you have to guess
        ' what number goes into a cell to complete the puzzle.  I feel that guessing
        ' takes away from the core essense of the game which is to solve the puzzle 
        ' by pure logic.  Of course, this is debatable among Sudoku game enthusiates.
        ' But for this program, each game that is generated can have one and only one
        ' solution.
        '
        ' After a Sudoku game is generated, we need to mask some cells based on the
        ' level of difficulty requested.  But we need to make sure that after
        ' masking, the puzzle has one and only one solution.
        '
        ' At first, I thought of the following logic:
        '   
        '   CreateGame()
        '   Do
        '       MaskPuzzle()
        '   Loop Until number of solution to puzzle = 1
        '
        ' But after looking at that, I realize that after creating a puzzle, there
        ' really is one and only one solution.  So, in the process of masking the
        ' puzzle, we should try to solve the puzzle before masking the next cell.
        ' This turns out to be much faster since we can backtrack if masking the
        ' next cell creates a puzzle with multiple solutions.  So the code for
        ' the logic.
        '

#Region " Variables, Constants, And other Declarations "

#Region " Constants "

        ' Maximum number of iterations we will try before declaring the puzzle unsolvable.
        Private Const _cMaxIterations As Int32 = 10

#End Region

#Region " Variables "

        Private _eLevel As GameLevelEnum
        Private _bNotGood As Boolean

#End Region

#End Region

#Region " Properties "

#Region " Public Properties "

        Friend ReadOnly Property NotGood As Boolean
            Get
                Return _bNotGood
            End Get
        End Property

#End Region

#Region " Private Properties "

        Private ReadOnly Property Level As GameLevelEnum
            Get
                Return _eLevel
            End Get
        End Property

#End Region

#End Region

#Region " Constructors "

        Friend Sub New(Level As GameLevelEnum)
            _eLevel = Level
        End Sub

#End Region

#Region " Methods "

#Region " Public Methods "

        Friend Sub MaskPuzzle(uCells(,) As CellStateClass)
            Dim uList As List(Of CellStateClass) = TransferToList(uCells)           ' Transfer 2D array to list
            Dim iMask As Int32 = 81 - GetMaskValue(Level)                           ' Figure out number of squares to mask
            Dim cSolve As New SolveGame()                                           ' Instantiate the Solve class
            Dim iNumIterations As Int32 = 0                                         ' Init the iteration counter
            _bNotGood = False                                                       ' Clear the NotGood flag
            Do
                Dim uIndex1 As CellIndex = FindRandomCell(uList)                    ' Find a random cell to mask
                Dim uIndex2 As CellIndex = GetMirror(uIndex1)                       ' Get the mirror coordinates
                SetCellState(uCells, uIndex1, uIndex2, CellStateEnum.Blank)         ' Set the state to "Blank"
                cSolve.SolvePuzzle(uCells)                                          ' Try to solve the puzzle
                If cSolve.Solvable Then                                             ' Is it solvable?
                    iMask -= RemoveCells(uList)                                     ' Yes, remove masked cells from list
                    iNumIterations = 0                                              ' Reset iteration counter
                Else                                                                ' Couldn't solve it
                    SetCellState(uCells, uIndex1, uIndex2, CellStateEnum.Answer)    ' Restore the cells back to Answer
                    iNumIterations += 1                                             ' Increment the iteration counter
                    If iNumIterations > _cMaxIterations Then                        ' Did we hit the max number of iterations?
                        _bNotGood = True                                            ' Yes, raise flag
                        Exit Do                                                     ' Then exit Do loop
                    End If
                End If
            Loop Until iMask <= 0                                                   ' Keep looping until we unmask the necessary number of cells
        End Sub

#End Region

#Region " Private Methods: Shared Methods "

        Private Shared Function TransferToList(uCells(,) As CellStateClass) As List(Of CellStateClass)
            Dim uList As New List(Of CellStateClass)(81)                ' Instantiate a list with a capacity of 81 elements
            For iCol As Int32 = 1 To 9                                  ' Loop through list
                For iRow As Int32 = 1 To 9
                    uList.Add(uCells(iCol, iRow))                       ' Transfer 2D array to list
                Next
            Next
            Return uList
        End Function

        Private Shared Function GetMirror(uIndex As CellIndex) As CellIndex
            Dim iCol As Int32 = 10 - uIndex.Col                         ' Find mirror image along the vertical axis
            Return New CellIndex(iCol, uIndex.Row)
        End Function

        Private Shared Function FindRandomCell(uList As List(Of CellStateClass)) As CellIndex
            Dim iIndex As Int32 = RandomClass.GetRandomInt(uList.Count - 1) ' Find a random cell to mask
            Return uList(iIndex).CellIndex                                  ' Return the cell index of that element
        End Function

        Private Shared Function FindFirstMaskedCell(uList As List(Of CellStateClass)) As Int32
            For I As Int32 = 0 To uList.Count - 1                           ' Scan through the list
                If uList(I).CellState = CellStateEnum.Blank Then            ' Find the first Blank state
                    Return I                                                ' Return that index
                End If
            Next
            Return -1                                                       ' Not found, return -1
        End Function

        Private Shared Sub SetCellState(uCells(,) As CellStateClass, uIndex1 As CellIndex, uIndex2 As CellIndex, eState As CellStateEnum)
            ' Set the states of the two cells
            With uIndex1
                uCells(.Col, .Row).CellState = eState
            End With
            With uIndex2
                uCells(.Col, .Row).CellState = eState
            End With
        End Sub

        Private Shared Function RemoveCells(uList As List(Of CellStateClass)) As Int32
            Dim iIndex As Int32 = FindFirstMaskedCell(uList)    ' Get index of first masked cell
            uList.RemoveAt(iIndex)                              ' Remove it
            Dim iCount As Int32 = 1                             ' Set the counter
            iIndex = FindFirstMaskedCell(uList)                 ' Find index of next masked cell
            If iIndex >= 0 Then                                 ' Valid index?
                uList.RemoveAt(iIndex)                          ' Yes, remove it too
                iCount += 1                                     ' Increment count
            End If                                              ' If index is invalid, it means we removed from center column (5)
            Return iCount                                       ' Return number of cells removed
        End Function

        ' Level             Given
        '=========================
        ' Very Easy         50+     
        ' Easy              36-49
        ' Medium            32-35
        ' Hard              28-31
        ' Expert            22-27

        Private Shared Function GetMaskValue(eLevel As GameLevelEnum) As Int32
            Dim iMin As Int32
            Dim iMax As Int32
            Select Case eLevel
                Case GameLevelEnum.VeryEasy
                    ' Very easy puzzles have between 50 and 60 given values
                    iMin = 50
                    iMax = 60

                Case GameLevelEnum.Easy
                    ' Easy puzzles have between 36 and 49 given values
                    iMin = 36
                    iMax = 49

                Case GameLevelEnum.Medium
                    ' Medium puzzles between 32 and 35 given values
                    iMin = 32
                    iMax = 35

                Case GameLevelEnum.Hard
                    ' Hard puzzles have between 28 and 31 given values
                    iMin = 28
                    iMax = 31

                Case Else   ' Expert level
                    ' Expert puzzles have between 22 and 27 given values
                    iMin = 22
                    iMax = 27

            End Select
            Return RandomClass.GetRandomInt(iMin, iMax)
        End Function

#End Region

#End Region

    End Class

End Namespace