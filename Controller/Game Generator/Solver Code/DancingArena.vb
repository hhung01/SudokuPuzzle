' Class implementing exact cover algorithm.
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

    Friend MustInherit Class DancingArena

        ''' <summary>
        ''' Root node with row = 0 and column = 0, pointing to
        ''' column header list.
        ''' </summary>
        ''' <remarks></remarks>
        Private _root As DancingColumn
        ''' <summary>
        ''' Total number of rows in matrix. Does not play any role
        ''' during solving, just for info.
        ''' </summary>
        ''' <remarks></remarks>
        Private _rows As Integer = 0
        ''' <summary>
        ''' Total number of columns in matrix. Does not play any role
        ''' during solving, just for info.
        ''' </summary>
        ''' <remarks></remarks>
        Private _columns As Integer = 0
        ''' <summary>
        ''' Index of first solution row. Used when some solution
        ''' rows are given.
        ''' </summary>
        ''' <remarks></remarks>
        Private _initial As Integer = 0
        ''' <summary>
        ''' Array holding solution rows with doubly linked lists.
        ''' Array size is defined when creating new arena and
        ''' equals to number of primary columns.
        ''' </summary>
        ''' <remarks></remarks>
        Private _solutionRows() As DancingNode
        ''' <summary>
        ''' Array for pointers to all column headers. Used for easier
        ''' scan over all the columns.
        ''' </summary>
        ''' <remarks></remarks>
        Private _headerColumns() As DancingColumn
        ''' <summary>
        ''' Counter of node updates during solving. Used for evaluation
        ''' of algorithm speed and has no role in solving.
        ''' </summary>
        ''' <remarks></remarks>
        Private _updates As Long = 0

        ''' <summary>
        ''' Create header with circular doubly linked list of
        ''' column headers for the matrix and root element
        ''' pointing to the leftmost and rightmost column header.
        ''' </summary>
        ''' <param name="primary">
        ''' Number of primary columns in matrix.
        ''' </param>
        ''' <param name="secondary">
        ''' Number of secondary columns in matrix.
        ''' </param>
        ''' <remarks></remarks>
        Friend Sub New(ByVal primary As Integer, ByVal secondary As Integer)
            _root = New DancingColumn(0)
            _rows = 0
            _columns = primary + secondary
            _initial = 0
            _updates = 0
            'Only primary columns form solution.
            ReDim _solutionRows(primary - 1)
            ReDim _headerColumns(primary + secondary - 1)
            For i As Integer = 0 To primary - 1
                _solutionRows(i) = Nothing
                _headerColumns(i) = New DancingColumn(i + 1)
                _headerColumns(i).Right = Nothing
                If (i > 0) Then
                    'Connecting column to it's neighbours.
                    _headerColumns(i).Left = _headerColumns(i - 1)
                    _headerColumns(i - 1).Right = _headerColumns(i)
                End If
            Next
            'Connecting first and last primary column to root node.
            _headerColumns(0).Left = _root
            _root.Right = _headerColumns(0)
            _headerColumns(primary - 1).Right = _root
            _root.Left = _headerColumns(primary - 1)
            'Adding self referential secondary columns.
            If secondary > 0 Then
                For i As Integer = 0 To secondary - 1
                    _headerColumns(primary + i) = New DancingColumn(primary + i + 1)
                Next
            End If
        End Sub

        ''' <summary>
        ''' Create header with circular doubly linked list of
        ''' column headers for the matrix and root element
        ''' pointing to the leftmost column header.
        ''' </summary>
        ''' <param name="columns">
        ''' Number of primary columns in matrix.
        ''' </param>
        ''' <remarks></remarks>
        Friend Sub New(ByVal columns As Integer)
            Me.New(columns, 0)
        End Sub

        ''' <summary>
        ''' Get root element.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private ReadOnly Property Root() As DancingColumn
            Get
                Return _root
            End Get
        End Property

        ''' <summary>
        ''' Get leftmost column header.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private ReadOnly Property FirstColumn() As DancingColumn
            Get
                Return CType(_root.Right(), DancingColumn)
            End Get
        End Property

        ''' <summary>
        ''' Get rightmost column header.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend ReadOnly Property LastColumn() As DancingColumn
            Get
                Return CType(_root.Left(), DancingColumn)
            End Get
        End Property

        ''' <summary>
        ''' Return total number of rows in matrix.
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
        ''' Get total number of columns in matrix.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend ReadOnly Property Columns() As Integer
            Get
                Return _columns
            End Get
        End Property

        ''' <summary>
        ''' Get total number of updates, ie number of times program
        ''' covered or uncovered some node during solving.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend ReadOnly Property Updates() As Long
            Get
                Return _updates
            End Get
        End Property

        ''' <summary>
        ''' Implements Knuth's algorithm for covering the column.
        ''' </summary>
        ''' <param name="column"></param>
        ''' <remarks></remarks>
        Private Sub CoverColumn(ByVal column As DancingColumn)
            'Exclude column header node from the list.
            _updates = _updates + 1
            column.Left().Right = column.Right()
            column.Right().Left = column.Left()
            'Now do for each row in excluded column...
            Dim row As DancingNode = column.Lower()
            While (Equals(row, column) = False)
                '... cover all nodes in a row...
                Dim col As DancingNode = row.Right()
                While (Equals(col, row) = False)
                    '... by excluding nodes from their columns.
                    _updates = _updates + 1
                    col.Upper().Lower = col.Lower()
                    col.Lower().Upper = col.Upper()
                    col.Header().DecRows()
                    col = col.Right()
                End While
                row = row.Lower()
            End While
        End Sub

        ''' <summary>
        ''' Implements Knuth's algorithm for uncovering the column.
        ''' </summary>
        ''' <param name="column"></param>
        ''' <remarks></remarks>
        Private Sub UncoverColumn(ByVal column As DancingColumn)
            'For each row in excluded column...
            Dim row As DancingNode = column.Upper()
            While (Equals(row, column) = False)
                '... uncover all nodes in a row...
                Dim col As DancingNode = row.Left()
                While (Equals(col, row) = False)
                    '... by connecting nodes to their columns.
                    _updates = _updates + 1
                    col.Upper().Lower = col
                    col.Lower().Upper = col
                    col.Header().IncRows()
                    col = col.Left()
                End While
                row = row.Upper()
            End While
            'Return column header node to the list.
            _updates = _updates + 1
            column.Left().Right = column
            column.Right().Left = column
        End Sub

        ''' <summary>
        ''' Add row of circular doubly linked nodes defined with
        ''' their column positions.
        ''' </summary>
        ''' <param name="positions"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function AddRow(ByVal positions() As Integer) As DancingNode
            Dim result As DancingNode = Nothing
            If positions.Length() > 0 Then
                Dim found As Boolean 'result of searching of column
                Dim thisNode As DancingNode
                Dim prevNode As DancingNode = Nothing
                _rows = _rows + 1 'counter of number of rows in matrix
                For i As Integer = 0 To positions.Length() - 1
                    If ((IsNothing(positions(i)) = False) And (positions(i) > 0)) Then
                        'Create new node.
                        thisNode = New DancingNode(_rows, positions(i))
                        'Connect new node to the left node in the same row.
                        thisNode.Left = prevNode
                        thisNode.Right = Nothing
                        If (IsNothing(prevNode) = False) Then
                            'Not the first node in row,
                            prevNode.Right = thisNode
                        Else
                            'This is first node in this row.
                            result = thisNode
                        End If
                        'Search for column with corresponding column number.
                        found = False
                        For Each col As DancingColumn In _headerColumns
                            If col.Column = positions(i) Then
                                'Column header found. Remember this header.
                                thisNode.Header = col
                                'Add new node to column we found. Column class will
                                'provide connections to upper and lower nodes.
                                col.AddNode(thisNode)
                                found = True
                            End If
                        Next
                        If (found = False) Then
                            Console.WriteLine("Can't find header for {0}", positions(i))
                        End If
                        'Remember new node is last added.
                        prevNode = thisNode
                        'Make circular connection.
                        result.Left = prevNode
                        prevNode.Right = result
                    End If
                Next
            End If
            Return result
        End Function

        ''' <summary>
        ''' Select next column to cover. Finds column with minimum rows.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function NextColumn() As DancingColumn
            Dim result As DancingColumn = CType(_root.Right(), DancingColumn)
            Dim minRows As Integer = result.Rows()
            Dim scaner As DancingColumn = CType(_root.Left(), DancingColumn)
            Do While (Equals(scaner, _root.Right()) = False)
                If scaner.Rows() < minRows Then
                    result = scaner
                    minRows = scaner.Rows()
                End If
                scaner = CType(scaner.Left(), DancingColumn)
            Loop
            Return result
        End Function

        ''' <summary>
        ''' Implements Knuth's DLX algorithm.
        ''' </summary>
        ''' <param name="index"></param>
        ''' <remarks></remarks>
        Private Sub SolveRecurse(ByVal index As Integer)
            If (Equals(_root, _root.Right()) = True) Then
                'No more columns, we found one of solutions.
                HandleSolution(_solutionRows)
            Else
                'Select next column using some selection algorithm.
                Dim nextCol As DancingColumn = NextColumn()
                'Exclude selected column from the matrix.
                CoverColumn(nextCol)
                'Try for each row in selected column...
                Dim row As DancingNode = nextCol.Lower()
                While (Equals(row, nextCol) = False)
                    'Add row to solution array.
                    _solutionRows(index) = row
                    '... exclude all columns covered by this row ...
                    Dim col As DancingNode = row.Right()
                    While (Equals(col, row) = False)
                        CoverColumn(col.Header())
                        col = col.Right()
                    End While
                    '... and try to solve reduced matrix.
                    SolveRecurse(index + 1)
                    'Now restore all columns covered by this row ...
                    col = row.Left()
                    While (Equals(col, row) = False)
                        UncoverColumn(col.Header())
                        col = col.Left()
                    End While
                    '... and remove row from solution array.
                    _solutionRows(index) = Nothing
                    row = row.Lower()
                End While
                'Return excluded column back to list.
                UncoverColumn(nextCol)
            End If
        End Sub

        ''' <summary>
        ''' Start recursive solving with initial step.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub Solve()
            _updates = 0
            SolveRecurse(_initial)
        End Sub

        ''' <summary>
        ''' Remove known rows (partial solution known or initial position).
        ''' Global counter of solution rows is used so partial adding of
        ''' solution rows is possible. Care should be taken not to add
        ''' the same solution row twice as space in solution rows array will
        ''' be ocupied and then all solutions will not fit into array
        ''' resulting in overflow.
        ''' </summary>
        ''' <param name="solutions"></param>
        ''' <remarks></remarks>
        Friend Sub RemoveKnown(ByVal solutions As Collection)

            Dim row As DancingNode
            For Each row In solutions
                _solutionRows(_initial) = row
                _initial = _initial + 1
                CoverColumn(row.Header())
                Dim col As DancingNode = row.Right()
                While (Equals(col, row) = False)
                    CoverColumn(col.Header())
                    col = col.Right()
                End While
            Next
        End Sub

        ''' <summary>
        ''' Childs of this class must implement algorithm for
        ''' handling result.
        ''' </summary>
        ''' <param name="rows"></param>
        ''' <remarks></remarks>
        Friend MustOverride Sub HandleSolution(ByVal rows() As DancingNode)

        ''' <summary>
        ''' For testing purpose only, shows all columns and nodes
        ''' with connections.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub ShowState()
            Dim col As DancingColumn = FirstColumn()
            While (Equals(col, Root()) = False)
                Console.WriteLine("C : {0}", col.ToString())
                Dim row As DancingNode = col.Lower()
                While (Equals(row, col) = False)
                    Console.WriteLine("   R : {0}", row.ToString())
                    row = row.Lower()
                End While
                col = CType(col.Right(), DancingColumn)
            End While
        End Sub

    End Class

End Namespace
