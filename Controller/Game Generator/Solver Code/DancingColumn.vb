' Class implementing column header.
' 
' Copyright (c) 2006 Miran Uhan
' 
' This program is free software; you can redistribute it and/or modify
' it under the terms of the GNU General Friend License as published by
' the Free Software Foundation; either version 2 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Friend License for more details.
'
' You should have received a copy of the GNU General Friend License
' along with this program; if not, write to the
'    Free Software Foundation, Inc.
'    51 Franklin Street, Fifth Floor
'    Boston, MA 02110-1301 USA
' 
' Resources
' - Donald E. Knuth "Dancing Links" (see
'   http://www-cs-faculty.stanford.edu/~knuth/preprints.html).
' - Some ideas I got studying Stan Chesnutt java implementation (see
'   http://www.bluechromis.com:8080/stan/chesnutt.html).
'
' For questions you can contact author at
'    miran.uhan@gmail.com
' Sugestions and bug reports are also welcome.
' 

Namespace Controller.GameGenerator

    Friend Class DancingColumn
        Inherits DancingNode

        ''' <summary>
        ''' Counter of nodes in this column.
        ''' </summary>
        ''' <remarks></remarks>
        Private _rows As Integer

        ''' <summary>
        ''' Create self-referential node with row number 0.
        ''' </summary>
        ''' <param name="column"></param>
        ''' <remarks></remarks>
        Friend Sub New(ByVal column As Integer)
            MyBase.New(0, column)
        End Sub

        ''' <summary>
        ''' Get number of nonzero rows in this column.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend ReadOnly Property Rows() As Integer
            Get
                Return _rows
            End Get
        End Property

        ''' <summary>
        ''' Increase number of rows for 1.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub IncRows()
            _rows = _rows + 1
        End Sub

        ''' <summary>
        ''' Decrease number of rows for 1.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub DecRows()
            _rows = _rows - 1
        End Sub

        ''' <summary>
        ''' Return string with this node name and descriptors
        ''' with names of all neighbours and header and number
        ''' of rows in this column.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return MyBase.ToString() & ", rows " & _rows
        End Function

        ''' <summary>
        ''' Add new row node to the end of column. Sets links
        ''' of this node, previously last node and column
        ''' header to form circular list.
        ''' </summary>
        ''' <param name="node"></param>
        ''' <remarks></remarks>
        Friend Sub AddNode(ByVal node As DancingNode)
            Dim last As DancingNode = Me.Upper()
            node.Upper = last
            node.Lower = Me
            last.Lower = node
            Me.Upper = node
            _rows = _rows + 1
        End Sub

    End Class

End Namespace