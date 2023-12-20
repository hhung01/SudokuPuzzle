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

    Friend Class PopulateCells

#Region " Variables "

        Private _uCells(80) As CellStateClass       ' A one dimensional array of cells that represent the 9x9 board

#End Region

#Region " Public Methods "

        Friend Function GeneratePuzzle() As CellStateClass(,)
            Clear()                                 ' Clear the current board
            GenerateGrid()                          ' Now generate a new puzzle
            Return TransferGameToGrid()             ' Transfer the array to the play grid
        End Function

#End Region

#Region " Private Methods "

        Private Sub Clear()
            For I As Int32 = 0 To 80                ' Loop through all the elements in the array
                _uCells(I) = Nothing                ' Set it to nothing
            Next
        End Sub

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
        '
        ' Here's a diagram showing how the 9x9 grid is indexed as a single dimensional array from 0 to 80
        '   +--------+--------+--------+
        '   |00 01 02|03 04 05|06 07 08|
        '   |09 10 11|12 13 14|15 16 17|
        '   |18 19 20|21 22 23|24 25 26|
        '   +--------+--------+--------+
        '   |27 28 29|30 31 32|33 34 35|
        '   |36 37 38|39 40 41|42 43 44|
        '   |45 46 47|48 49 50|51 52 53|
        '   +--------+--------+--------+
        '   |54 55 56|57 58 59|60 61 62|
        '   |63 64 65|66 67 68|69 70 71|
        '   |72 73 74|75 76 77|78 79 80|
        '   +--------+--------+--------+

        Private Sub GenerateGrid()
            Dim lAvailable() As List(Of Int32) = InitializeArray()                  ' An array of lists of integers:  we use this to keep track of what numbers we can still use in what cell

            ' Create the board
            Dim iIndex As Int32 = 0                                                 ' Use this to count the cell we are up to: 0 through 80
            Do                                                                      ' Now loop through the grid
                If lAvailable(iIndex).Count > 0 Then                                ' Are there any more numbers to try?
                    Dim I As Int32 = RandomClass.GetRandomInt(lAvailable(iIndex).Count) ' Yes, pick a random value from the list of available values for this cell
                    Dim uIndex As CellIndex = New CellIndex(iIndex)
                    Dim Item As New CellStateClass(uIndex, lAvailable(iIndex).Item(I))
                    If Conflicts(Item) Then                                             ' Check if the value conflicts
                        lAvailable(iIndex).RemoveAt(I)                                  ' Yes, so we remove it from the list and try again
                        Item = Nothing                                                  ' Discard the item
                    Else
                        _uCells(iIndex) = Item                                          ' No, so save it
                        lAvailable(iIndex).RemoveAt(I)                                  ' Also remove it from the cell's list of valid values for future reference, just in case
                        iIndex += 1                                                     ' Move to the next cell
                    End If
                Else                                                                    ' No more numbers to pick for this cell
                    For Y As Int32 = 1 To 9                                             ' So, reset all available numbers
                        lAvailable(iIndex).Add(Y)
                    Next
                    iIndex -= 1                                                         ' Go back to the previous square
                    _uCells(iIndex) = Nothing                                           ' Clear the cell and try again
                End If
            Loop Until iIndex = 81                                                      ' Keep looping until we're done with all 81 cells
        End Sub

        Private Shared Function InitializeArray() As List(Of Int32)()
            Dim lAvailable(80) As List(Of Int32)                    ' An array of lists of integers:  we use this to keep track of what numbers we can still use in what cell

            ' Initialize each element of the array with a list of numbers from 1 through 9
            ' Meaning, each of the 81 cells will have a valid value from 1 to 9 to start with
            For I As Int32 = 0 To 80
                lAvailable(I) = New List(Of Integer)
                For J As Integer = 1 To 9
                    lAvailable(I).Add(J)
                Next
            Next
            Return lAvailable
        End Function

        Private Function Conflicts(ByVal uCheck As CellStateClass) As Boolean
            ' Returns True of there is a matching value in the existing row or column or region
            ' Return False otherwise
            For Each Item As CellStateClass In _uCells
                If (Item IsNot Nothing) Then
                    With Item
                        If .IsSameCol(uCheck) OrElse .IsSameRow(uCheck) OrElse .IsSameRegion(uCheck) Then
                            If (.Answer = uCheck.Answer) Then
                                Return True
                            End If
                        End If
                    End With
                End If
            Next
            Return False
        End Function

        Private Function TransferGameToGrid() As CellStateClass(,)
            ' Done generating a game.  Now transfer the one dimensional array
            ' to a two dimensional (9x9) grid.
            Dim Cells(9, 9) As CellStateClass
            For Each Item As CellStateClass In _uCells
                Cells(Item.Col, Item.Row) = Item
            Next
            Return Cells
        End Function

#End Region

    End Class

End Namespace
