' Class implementing matrix "1" element as data object.
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

    Friend Class DancingNode

        ''' <summary>
        ''' Pointer to the left node.
        ''' </summary>
        ''' <remarks></remarks>
        Private _left As DancingNode
        ''' <summary>
        ''' Pointer to the right node.
        ''' </summary>
        ''' <remarks></remarks>
        Private _right As DancingNode
        ''' <summary>
        ''' Pointer to the upper node.
        ''' </summary>
        ''' <remarks></remarks>
        Private _upper As DancingNode
        ''' <summary>
        ''' Pointer to the lower node.
        ''' </summary>
        ''' <remarks></remarks>
        Private _lower As DancingNode
        ''' <summary>
        ''' Pointer to the column header.
        ''' </summary>
        ''' <remarks></remarks>
        Private _header As DancingColumn
        ''' <summary>
        ''' Row number of this node. Only used for creating name,
        ''' has no role in solving.
        ''' </summary>
        ''' <remarks></remarks>
        Private _row As Integer
        ''' <summary>
        ''' Column number of this node. Only used for creating name,
        ''' has no role in solving.
        ''' </summary>
        ''' <remarks></remarks>
        Private _column As Integer

        ''' <summary>
        ''' Create self-referential node.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <param name="column"></param>
        ''' <remarks></remarks>
        Friend Sub New(ByVal row As Integer, ByVal column As Integer)
            _left = Me
            _right = Me
            _upper = Me
            _lower = Me
            _header = Nothing
            _row = row
            _column = column
        End Sub

        ''' <summary>
        ''' Get or set node being left to this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Left() As DancingNode
            Get
                Return _left
            End Get
            Set(ByVal value As DancingNode)
                _left = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set node being right to this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Right() As DancingNode
            Get
                Return _right
            End Get
            Set(ByVal value As DancingNode)
                _right = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set node being upper to this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Upper() As DancingNode
            Get
                Return _upper
            End Get
            Set(ByVal value As DancingNode)
                _upper = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set node being lower to this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Lower() As DancingNode
            Get
                Return _lower
            End Get
            Set(ByVal value As DancingNode)
                _lower = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set header node for this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Header() As DancingColumn
            Get
                Return _header
            End Get
            Set(ByVal value As DancingColumn)
                _header = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set row number for this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Row() As Integer
            Get
                Return _row
            End Get
            Set(ByVal value As Integer)
                _row = value
            End Set
        End Property

        ''' <summary>
        ''' Get or set column number for this node.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Property Column() As Integer
            Get
                Return _column
            End Get
            Set(ByVal value As Integer)
                _column = value
            End Set
        End Property

        ''' <summary>
        ''' For testing purpose only. Verifies weather node fits into
        ''' this row and this column.
        ''' </summary>
        ''' <param name="row"></param>
        ''' <param name="column"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function Verify(ByVal row As Integer, _
           ByVal column As Integer) As Boolean
            Return (row = _row) And (column = _column)
        End Function

        ''' <summary>
        ''' Return name of node in form "Row r#, column c#"
        ''' or "NULL" if node is not set.
        ''' </summary>
        ''' <param name="node"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Name(ByVal node As DancingNode) As String
            If (IsNothing(node) = True) Then
                Return "NULL"
            Else
                Return node.Name()
            End If
        End Function

        ''' <summary>
        ''' Return name of this node in form "Row r#, column c#"
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Name() As String
            Return "row" & _row & ", column" & _column
        End Function

        ''' <summary>
        ''' Return string with this node name and descriptors
        ''' with names of all neighbour nodes and header.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return "Node(" & Name() & _
                   "), left(" & Name(Left()) & _
                   "), right(" & Name(Right()) & _
                   "), upper(" & Name(Upper()) & _
                   "), lower(" & Name(Lower()) & _
                   "), header(" & Name(Header()) & ")"
        End Function

    End Class

End Namespace