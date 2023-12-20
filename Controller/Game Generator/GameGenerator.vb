'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Imports System.Threading
Imports SudokuPuzzle.Model

Namespace Controller.GameGenerator

    Friend Class GameGenerator
        ' We use a background thread to create the new game because
        ' we don't want to hold up other threads from processing.

#Region " Variables, Constants, And other Declarations "

#Region " Variables "

        Private _eLevel As GameLevelEnum

#End Region

#Region " Other Declarations "

        Friend Delegate Sub GameGeneratorEventHandler(sender As Object, e As GameGeneratorEventArgs)
        Friend Event GameGeneratorEvent As GameGeneratorEventHandler

#End Region

#End Region

#Region " Constructors "

        Friend Sub New(Level As GameLevelEnum)
            _eLevel = Level
        End Sub

#End Region

#Region " Public Methods "

        Friend Sub CreateNewGame()
            ' Start a background thread to create a new game
            Dim tThread As New Thread(AddressOf GenerateNewGame)    ' Define a new thread
            tThread.IsBackground = True                             ' Set it as a background thread
            tThread.Start()                                         ' Start it
        End Sub

#End Region

#Region " Private Methods "

        Private Sub GenerateNewGame()
            Dim uCells(,) As CellStateClass = GenerateNewBoard()        ' Create a new puzzle
            Dim e As GameGeneratorEventArgs = New GameGeneratorEventArgs(uCells)
            RaiseEvent GameGeneratorEvent(Me, e)
        End Sub

        Friend Function GenerateNewBoard() As CellStateClass(,)
            Dim uCells(,) As CellStateClass
            Dim cPopulate As New PopulateCells
            Dim cMask As MaskPuzzle = New MaskPuzzle(_eLevel)
            Do
                uCells = cPopulate.GeneratePuzzle()                 ' Generate a puzzle
                cMask.MaskPuzzle(uCells)                            ' Mask the appropriate number of cells given the level
            Loop While cMask.NotGood                                ' Was it successful?  No, generate another puzzle and try again.  Yes, exit
            Return uCells
        End Function

#End Region

    End Class

End Namespace