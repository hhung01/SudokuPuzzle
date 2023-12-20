'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Imports System.Text
Imports SudokuPuzzle.SharedFunctions

Namespace Model

    Friend Class CellStateClass

#Region " Variables "

        Private _iAnswer As Int32
        Private _uCellIndex As CellIndex
        Private _bInvalidState As Boolean
        Private _bNotes(9) As Boolean

#End Region

#Region " Public Properties "

#Region " Read/Write Properties "

        Friend Property CellState As CellStateEnum
        Friend Property UserAnswer As Int32

        Friend Property Notes(iIndex As Int32) As Boolean
            Get
                If Common.IsValidIndex(iIndex) Then
                    Return _bNotes(iIndex)
                End If
                Return False
            End Get
            Set(value As Boolean)
                If Common.IsValidIndex(iIndex) Then
                    _bNotes(iIndex) = value
                End If
            End Set
        End Property

        Friend ReadOnly Property Notes() As Boolean()
            Get
                Return _bNotes
            End Get
        End Property

#End Region

#Region " Readonly Properties "

        Friend ReadOnly Property Answer As Int32
            Get
                Return _iAnswer
            End Get
        End Property

        Friend ReadOnly Property Row As Int32
            Get
                Return _uCellIndex.Row
            End Get
        End Property

        Friend ReadOnly Property Col As Int32
            Get
                Return _uCellIndex.Col
            End Get
        End Property

        Friend ReadOnly Property Region As Int32
            Get
                Return _uCellIndex.Region
            End Get
        End Property

        Friend ReadOnly Property InvalidState As Boolean
            Get
                Return _bInvalidState
            End Get
        End Property

        Friend ReadOnly Property CellIndex As CellIndex
            Get
                Return _uCellIndex
            End Get
        End Property

#End Region

#End Region

#Region " Constructors "

        Friend Sub New(iCol As Int32, iRow As Int32, sState As String)
            InitCell(iCol, iRow)
            LoadState(sState)
        End Sub

        Friend Sub New(iIndex As CellIndex, iAnswer As Int32)
            Initcell()
            _uCellIndex = iIndex
            _iAnswer = iAnswer
            CellState = CellStateEnum.Answer
        End Sub

#End Region

#Region " Public Methods "

        Friend Function IsCorrect() As Boolean
            Return Answer = UserAnswer                          ' Return true if user's answer matches our answer
        End Function

        Friend Function HasNotes() As Boolean
            For I As Int32 = 1 To 9                             ' Loop through the notes
                If _bNotes(I) Then                              ' If a note is raised?
                    Return True                                 ' Return true
                End If
            Next
            Return False                                        ' No notes marked, return False
        End Function

        Friend Shadows Function ToString(bFull As Boolean) As String
            Dim sTemp As New StringBuilder
            sTemp.Append(Answer.ToString)                       ' Append the answer
            sTemp.Append(CellState.GetHashCode.ToString)        ' Append the cell state
            If bFull Then                                       ' Return Full state?
                sTemp.Append(UserAnswer.ToString)               ' Yes, also include the user's answer
            End If
            Return sTemp.ToString                               ' Return the string
        End Function

        Friend Sub ClearNotes()
            For I As Int32 = 1 To 9                             ' Loop through the notes
                _bNotes(I) = False                              ' Clear the note
            Next
        End Sub

        Friend Function IsSameRow(uCell As CellStateClass) As Boolean
            If (uCell IsNot Nothing) Then
                If (uCell.Row <> 0) Then
                    Return _uCellIndex.IsSameRow(uCell.CellIndex)
                End If
            End If
            Return False
        End Function

        Friend Function IsSameCol(uCell As CellStateClass) As Boolean
            If (uCell IsNot Nothing) Then
                If (uCell.Col <> 0) Then
                    Return _uCellIndex.IsSameCol(uCell.CellIndex)
                End If
            End If
            Return False
        End Function

        Friend Function IsSameRegion(uCell As CellStateClass) As Boolean
            If (uCell IsNot Nothing) Then
                If (uCell.Region <> 0) Then
                    Return _uCellIndex.IsSameRegion(uCell.CellIndex)
                End If
            End If
            Return False
        End Function

#End Region

#Region " Private Methods "

        Private Sub Initcell()
            _iAnswer = 0
            UserAnswer = 0
            CellState = CellStateEnum.Blank
            ClearNotes()
        End Sub

        Private Sub InitCell(iCol As Int32, iRow As Int32)
            Initcell()
            _uCellIndex = New CellIndex(iCol, iRow)
        End Sub

        Private Sub LoadState(sState As String)
            _bInvalidState = True                                               ' Default state to True
            If (sState.Length >= 2) Then                                        ' String is at least 2 characters long?
                If ExtractAnswer(sState.Substring(0, 1)) Then                   ' Extract Answer
                    If ExtractCellState(sState.Substring(1, 1)) Then            ' Extract OK, extract cell state
                        If (sState.Length >= 3) Then                            ' Extract OK, more data?
                            If ExtractUserAnswer(sState.Substring(2, 1)) Then   ' Yes, extract user's answer
                                _bInvalidState = False                          ' Extract ok, set state to False
                            End If
                        Else    ' For input strings that are only 2 chars long, 
                            '     the only valid cell states are Answer and Blank.
                            _bInvalidState = Not ((CellState = CellStateEnum.Answer) OrElse (CellState = CellStateEnum.Blank))
                        End If
                    End If
                End If
            End If
        End Sub

        Private Function ExtractAnswer(sState As String) As Boolean
            _iAnswer = ExtractInt32(sState)                         ' Convert input to an integer
            Return (_iAnswer <> 0)                                  ' Return true if non-zero
        End Function

        Private Function ExtractUserAnswer(sState As String) As Boolean
            UserAnswer = ExtractInt32(sState)                       ' Convert input to an integer
            Return (UserAnswer <> 0)                                ' Return true if non-zero
        End Function

        Private Shared Function ExtractInt32(value As String) As Int32
            Dim iTemp As Int32
            Dim bTry As Boolean = Int32.TryParse(value, iTemp)      ' Try to convert string to an integer
            If bTry AndAlso Common.IsValidIndex(iTemp) Then         ' Valid conversion and within range?
                Return iTemp                                        ' Yes, return it
            End If
            Return 0                                                ' No, return zero
        End Function

        Private Function ExtractCellState(sState As String) As Boolean
            Try
                ' Use the Enum functions to convert the string to an enum
                Dim eState As CellStateEnum = CType([Enum].Parse(GetType(CellStateEnum), sState), CellStateEnum)
                If Common.IsValidStateEnum(eState) Then             ' Is it one of the valid states?
                    CellState = eState                              ' Yes, then save it.
                    Return True                                     ' Return True
                End If
            Catch ex As Exception
                ' TODO: What to do here?
                ' Maybe log the error into the Application.Event log?
            End Try
            CellState = CellStateEnum.Blank                         ' No, default state to "Blank"
            Return False                                            ' Return False
        End Function

#End Region

    End Class

End Namespace