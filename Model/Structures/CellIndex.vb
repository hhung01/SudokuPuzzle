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

    Friend Class CellIndex

#Region " Variables "

        Private _iRow As Int32 = -1
        Private _iCol As Int32 = -1
        Private _iRegion As Int32 = -1

#End Region

#Region " Public Properties "

        Friend ReadOnly Property Row As Int32
            Get
                Return _iRow
            End Get
        End Property
        Friend ReadOnly Property Col As Int32
            Get
                Return _iCol
            End Get
        End Property
        Friend ReadOnly Property Region As Int32
            Get
                Return _iRegion
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New(iCol As Int32, iRow As Int32)
            If Common.IsValidIndex(iCol, iRow) Then
                _iCol = iCol                                        ' Save column value
                _iRow = iRow                                        ' Save row value
                _iRegion = SetRegion(_iCol, _iRow)                  ' Set the region value
            End If
        End Sub

        Friend Sub New(iIndex As Int32)
            If (0 <= iIndex) AndAlso (iIndex <= 80) Then            ' Index valid?
                iIndex += 1                                         ' Yes, increment index
                _iCol = ComputeColumn(iIndex)                       ' Compute Column
                _iRow = ComputeRow(Me.Col, iIndex)                  ' Compute Row
                _iRegion = SetRegion(_iCol, _iRow)                  ' Set the region value
            End If
        End Sub

#End Region

#Region " Public Methods "

        Friend Function IsSameCell(uIndex As CellIndex) As Boolean
            If uIndex IsNot Nothing Then                                ' If the input object is not null
                Return ((Row = uIndex.Row) AndAlso (Col = uIndex.Col))  ' Return True if the Row And Col values are the same
            End If
            Return False                                                ' Otherwise, just return False
        End Function

        Friend Function IsSameRow(uIndex As CellIndex) As Boolean
            If uIndex IsNot Nothing Then                                ' If input object is not null
                Return (Row = uIndex.Row)                               ' Return True if row is the same
            End If
            Return False                                                ' Otherwise, return False
        End Function

        Friend Function IsSameCol(uIndex As CellIndex) As Boolean
            If uIndex IsNot Nothing Then                                ' If input object is not null
                Return (Col = uIndex.Col)                               ' Return True if column is same
            End If
            Return False                                                ' Otherwise return False
        End Function

        Friend Function IsSameRegion(uIndex As CellIndex) As Boolean
            If uIndex IsNot Nothing Then                                ' If input object is not null
                Return (Region = uIndex.Region)                         ' Return True if Region is the same
            End If
            Return False                                                ' Return False otherwise
        End Function

#End Region

#Region " Private Methods "

        Private Shared Function ComputeColumn(ByVal iIndex As Int32) As Int32
            Dim iRet As Integer = iIndex Mod 9
            If iRet = 0 Then
                Return 9
            End If
            Return iRet
        End Function

        Private Shared Function ComputeRow(iCol As Int32, ByVal iIndex As Int32) As Int32
            If iCol = 9 Then
                Return iIndex \ 9
            End If
            Return (iIndex \ 9) + 1
        End Function


        ' For reference, here is a diagram showing how each cell
        ' is referenced in code.  Basically, it's [col][row]
        ' much like how Excel worksheets do it.
        '   +--------+--------+--------+
        '   |11 21 31|41 51 61|71 81 91|
        '   |12 22 32|42 52 62|72 82 92|
        '   |13 23 33|43 53 63|73 83 93|
        '   +--------+--------+--------+
        '   |14 24 34|44 54 64|74 84 94|
        '   |15 25 35|45 55 65|75 85 95|
        '   |16 26 36|46 56 66|76 86 96|
        '   +--------+--------+--------+
        '   |17 27 37|47 57 67|77 87 97|
        '   |18 28 38|48 58 68|78 88 98|
        '   |19 29 39|49 59 69|79 89 99|
        '   +--------+--------+--------+
        '
        ' And these are how the regions are defined
        '   +--------+--------+--------+
        '   |.. .. ..|.. .. ..|.. .. ..|
        '   |..  1 ..|..  2 ..|..  3 ..|
        '   |.. .. ..|.. .. ..|.. .. ..|
        '   +--------+--------+--------+
        '   |.. .. ..|.. .. ..|.. .. ..|
        '   |..  4 ..|..  5 ..|..  6 ..|
        '   |.. .. ..|.. .. ..|.. .. ..|
        '   +--------+--------+--------+
        '   |.. .. ..|.. .. ..|.. .. ..|
        '   |..  7 ..|..  8 ..|..  9 ..|
        '   |.. .. ..|.. .. ..|.. .. ..|
        '   +--------+--------+--------+

        Private Shared Function SetRegion(iCol As Int32, iRow As Int32) As Int32
            If ((1 <= iRow) AndAlso (iRow <= 3)) Then
                If ((1 <= iCol) AndAlso (iCol <= 3)) Then
                    Return 1
                ElseIf ((4 <= iCol) AndAlso (iCol <= 6)) Then
                    Return 2
                Else
                    Return 3
                End If
            ElseIf ((4 <= iRow) AndAlso (iRow <= 6)) Then
                If ((1 <= iCol) AndAlso (iCol <= 3)) Then
                    Return 4
                ElseIf ((4 <= iCol) AndAlso (iCol <= 6)) Then
                    Return 5
                Else
                    Return 6
                End If
            Else
                If ((1 <= iCol) AndAlso (iCol <= 3)) Then
                    Return 7
                ElseIf ((4 <= iCol) AndAlso (iCol <= 6)) Then
                    Return 8
                Else
                    Return 9
                End If
            End If
        End Function

#End Region

    End Class

End Namespace