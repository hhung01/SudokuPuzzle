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

    Friend Class SolveGame

#Region " Variables "

        Private _bSolvable As Boolean

#End Region

#Region " Public Properties "

        Friend ReadOnly Property Solvable As Boolean
            Get
                Return _bSolvable
            End Get
        End Property

#End Region

#Region " Methods "

#Region " Public Methods "

        Friend Sub SolvePuzzle(uCells(,) As CellStateClass)
            Dim iTask(,) As Int32 = ConvertBoard(uCells)    ' Convert the cell class to a 9x9 array of integers
            Dim clsArena As New SudokuArena(iTask, 3, 3)    ' Instantiate the solver class.  The mini cells are 3x3
            clsArena.Solve()                                ' Solve it.
            _bSolvable = (clsArena.Solutions = 1)           ' Only one solution is considered solvable.
            ' If there are more than one, it means user has to guess and we don't want that.
        End Sub

#End Region

#Region " Private Methods "

        Private Function ConvertBoard(uCells(,) As CellStateClass) As Int32(,)
            Dim iRet(8, 8) As Int32
            For iRow As Int32 = 1 To 9
                For iCol As Int32 = 1 To 9
                    If uCells(iCol, iRow).CellState = CellStateEnum.Answer Then
                        iRet(iCol - 1, iRow - 1) = uCells(iCol, iRow).Answer
                    Else
                        iRet(iCol - 1, iRow - 1) = 0
                    End If
                Next
            Next
            Return iRet
        End Function

#End Region

#End Region

    End Class

End Namespace