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

Namespace Controller

    Friend Class GameGeneratorEventArgs
        Inherits EventArgs

#Region " Variables "

        Private _uCells(,) As CellStateClass

#End Region

#Region " Readonly Properties "

        Friend ReadOnly Property Cells As CellStateClass(,)
            Get
                Return _uCells
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New(uCells As CellStateClass(,))
            _uCells = uCells
        End Sub

#End Region

    End Class

End Namespace