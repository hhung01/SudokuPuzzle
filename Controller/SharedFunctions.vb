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

Namespace SharedFunctions

    Friend Class Common

        Friend Shared Function IsValidIndex(iIndex As Int32) As Boolean
            Return ((1 <= iIndex) AndAlso (iIndex <= 9))
        End Function

        Friend Shared Function IsValidIndex(iCol As Int32, iRow As Int32) As Boolean
            Return (IsValidIndex(iCol) AndAlso IsValidIndex(iRow))
        End Function

        Friend Shared Function IsValidIndex(uIndex As CellIndex) As Boolean
            If uIndex IsNot Nothing Then
                If (IsValidIndex(uIndex.Col, uIndex.Row)) Then
                    Return (IsValidIndex(uIndex.Region))
                End If
            End If
            Return False
        End Function

        Friend Shared Function IsValidStateEnum(value As Object) As Boolean
            Return [Enum].IsDefined(GetType(CellStateEnum), value)
        End Function

    End Class

End Namespace
