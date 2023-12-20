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
        ' TODO: Mask on a vertical mirror ... meaning the mask pattern is symmetrical
        '       on the vertical axis.
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
            Dim iList As List(Of CellStateClass) = TransferToList(uCells)       ' Transfer cell array to a list
            Dim iGiven As Int32 = GetMaskValue(Level)                           ' Figure out number of squares to mask
            Dim cSolve As New SolveGame()                                       ' Instantiate the Solve class
            Dim iNumIterations As Int32 = 0                                     ' Init the iteration counter
            _bNotGood = False                                                   ' Clear the NotGood flag
            Do
                Dim iIndex As Int32 = RandomClass.GetRandomInt(iList.Count - 1) ' Find a random cell to mask
                iList(iIndex).CellState = CellStateEnum.Blank                   ' Mark it as masked
                cSolve.SolvePuzzle(uCells)                                      ' Try to solve the puzzle
                If cSolve.Solvable Then                                         ' Is it solvable?
                    iList.RemoveAt(iIndex)                                      ' Yes, remove it from the list
                    iNumIterations = 0                                          ' Reset iteration counter
                Else
                    iList(iIndex).CellState = CellStateEnum.Answer              ' No, put cell state back to unmasked and try again
                    iNumIterations += 1                                         ' Increment the iteration counter
                    If iNumIterations > _cMaxIterations Then                    ' Did we hit the max number of iterations?
                        _bNotGood = True                                        ' Yes, raise flag
                        Exit Do                                                 ' Then exit Do loop
                    End If
                End If
            Loop Until iList.Count <= iGiven                                    ' Keep looping until we unmask the necessary number of squares to match the given level
        End Sub

#End Region

#Region " Private Methods "

#Region " Private Methods: Instance Methods "

        Private Function TransferToList(uCells(,) As CellStateClass) As List(Of CellStateClass)
            Dim iList As New List(Of CellStateClass)(81)
            For iCol As Int32 = 1 To 9
                For iRow As Int32 = 1 To 9
                    iList.Add(uCells(iCol, iRow))
                Next
            Next
            Return iList
        End Function

        Private Function GetMirror(iIndex As Int32) As Int32
            Dim uIndex As New CellIndex(iIndex)
            Dim iCol As Int32 = 10 - uIndex.Col
            Return (((uIndex.Row - 1) * 9) + iCol) - 1
        End Function

#End Region

#Region " Private Methods: Shared Methods "

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

#End Region

    End Class

End Namespace