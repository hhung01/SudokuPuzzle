'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Imports System.Threading
Imports System.Text
Imports SudokuPuzzle.Model

Namespace Controller.GameGenerator

    Friend Class GameCollection
        ' We use a background thread to manage the queue of games.
        ' This is so that any game creation or management of the games
        ' won't affect the UI.

#Region " Variables, Constants, And other Declarations "

#Region " Constants "

        Private Const _cDepth As Int32 = 5              ' Number of games to maintain per level

#End Region

#Region " Variables "

        Private _eLevel As GameLevelEnum
        Private _qGames As Queue(Of CellStateClass(,))
        Private _objQLock As Object
        Private _bStop As Boolean = False
        Private _MakeMoreGames As AutoResetEvent
        Private WithEvents _clsGameGenerator As GameGenerator

#End Region

#Region " Other Declarations "

        Friend Delegate Sub GameManagerEventHandler(sender As Object, e As GameManagerEventArgs)
        Friend Event GameManagerEvent As GameManagerEventHandler

#End Region

#End Region

#Region " Properties "

        Friend ReadOnly Property GameCount() As Int32
            Get
                SyncLock _objQLock                                  ' Acquire a lock on the Queue object
                    If _qGames Is Nothing Then                      ' Is queue object nothing?
                        _qGames = New Queue(Of CellStateClass(,))   ' Yes, create a new queue (should never happen)
                    End If
                    Return _qGames.Count                            ' Return the current count
                End SyncLock
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New(Level As GameLevelEnum)                      ' Initialize a new instance of this class with a fixed level
            InitializeVariables(Level)                              ' Init all the variables
        End Sub

#End Region

#Region " Methods "

#Region " Event Handlers "

        Private Sub GameGeneratorEvent(sender As Object, e As GameGeneratorEventArgs) Handles _clsGameGenerator.GameGeneratorEvent
            Dim iCount As Int32
            SyncLock _objQLock                                  ' Acquire a lock on the queue object
                If (_qGames Is Nothing) Then                    ' Is the queue initialized?
                    _qGames = New Queue(Of CellStateClass(,))   ' No, initialize it (should never happen)
                End If
                _qGames.Enqueue(e.Cells)                        ' Add new game to the queue
                iCount = _qGames.Count                          ' Update count
            End SyncLock
            UpdateCount(iCount)                                 ' Update the count
            _MakeMoreGames.Set()                                ' Tell background thread we're done
        End Sub

#End Region

#Region " Public Methods "

        Friend Sub StartThread()
            CreateGames()                                       ' Start background thread to create more games as necessary
        End Sub

        Friend Sub StopThread()
            _bStop = True                                       ' Raise the stop flag
            _MakeMoreGames.Set()                                ' Raise the mutex incase the background thread was waiting
        End Sub

        Friend Function GetGame() As CellStateClass(,)
            Try
                SyncLock _objQLock                                          ' Acquire a lock on the queue object
                    If _qGames Is Nothing Then                              ' Is Queue initialized?
                        _qGames = New Queue(Of CellStateClass(,))           ' No, initialize it (should never happen)
                    End If
                    If (_qGames.Count > 0) Then                             ' Anything in the queue?
                        Dim uCells(,) As CellStateClass = _qGames.Dequeue   ' Yes, pop a game off the queue
                        UpdateCount(_qGames.Count)                          ' Update count
                        _MakeMoreGames.Set()                                ' Tell the background thread to make more games
                        Return uCells                                       ' Return the game
                    End If
                End SyncLock
            Catch ex As Exception
                ' TODO: What to do here?
            End Try
            Return Nothing                                                  ' Something is wrong, return nothing
        End Function

        Friend Function SaveGames() As String
            SyncLock _objQLock                                      ' Obtain a lock on the queue
                If _qGames IsNot Nothing Then                       ' Is Queue initialized? (Should always be initialized.)
                    Dim sTemp As New StringBuilder                  ' Yes, then loop through the queue
                    For Each Item As CellStateClass(,) In _qGames
                        If Item IsNot Nothing Then                  ' Is this a valid game?
                            sTemp.Append(ConvertGameToString(Item)) ' Yes, then convert it to a string and append it
                        End If
                    Next
                    Return sTemp.ToString                           ' Return a string that represents all the games
                End If
            End SyncLock
            Return ""                                               ' Problem, just return a blank string
        End Function

        Friend Sub LoadGames(sGames As String)
            If Not String.IsNullOrWhiteSpace(sGames) Then
                SyncLock _objQLock
                    If _qGames Is Nothing Then                                          ' Is queue pointer null?
                        _qGames = New Queue(Of CellStateClass(,))                       ' Yes, assign a new queue (should never happen)
                    End If
                    Dim iPtr As Int32 = 0                                               ' Initialize the pointer to zero
                    Do While sGames.Length > iPtr                                       ' Keep looping while there are more games to process
                        Dim sTemp As String = sGames.Substring(iPtr, 162)               ' Grab a substring from the main list
                        Dim uCells(,) As CellStateClass = ConvertStringToGame(sTemp)    ' Convert it
                        If uCells IsNot Nothing Then                                    ' Conversion successful?
                            _qGames.Enqueue(uCells)                                     ' Yes, add it to the game queue
                        End If
                        uCells = Nothing                                                ' Clear the Cell pointer
                        iPtr += 162                                                     ' Move string pointer to the next game in the list
                    Loop
                    UpdateCount(_qGames.Count)                                          ' Update count
                End SyncLock                                                            ' Release the lock on the queue
            End If
        End Sub

#End Region

#Region " Private Methods "

        Private Sub InitializeVariables(Level As GameLevelEnum)
            _eLevel = Level
            _bStop = False
            _objQLock = New Object
            _MakeMoreGames = New AutoResetEvent(False)
            _clsGameGenerator = New GameGenerator(Level)
            SyncLock _objQLock
                _qGames = New Queue(Of CellStateClass(,))
            End SyncLock
        End Sub

        Private Sub CreateGames()
            Dim tThread As New Thread(AddressOf GameMaker)          ' Create a thread
            tThread.IsBackground = True                             ' Make it a background thread
            tThread.Start()                                         ' Start it
        End Sub

        Private Sub GameMaker()
            Do
                Try
                    SyncLock _objQLock                                  ' Acquire a lock on the queue object
                        If _qGames Is Nothing Then                      ' Is queue pointer null?
                            _qGames = New Queue(Of CellStateClass(,))   ' Yes, assign a new queue (should never happen)
                        End If
                        If _qGames.Count < _cDepth Then                 ' Need more games?
                            _clsGameGenerator.CreateNewGame()           ' Yes, create a new game for this level
                        End If
                    End SyncLock
                Catch ex As Exception
                    ' TODO: What to do here?
                End Try
                _MakeMoreGames.WaitOne()                                ' Wait for a signal to make more games
            Loop Until _bStop                                           ' Keep looping until the application closes
        End Sub

        Private Function ConvertGameToString(uCells As CellStateClass(,)) As String
            Dim sTemp As New StringBuilder
            For iCol As Int32 = 1 To 9                                  ' Loop through the columns
                For iRow As Int32 = 1 To 9                              ' Loop through the rows
                    sTemp.Append(uCells(iCol, iRow).ToString(False))    ' Convert each cell to a string representation
                Next
            Next
            Return sTemp.ToString                                       ' Return the game as string representation
        End Function

        Private Function ConvertStringToGame(sInput As String) As CellStateClass(,)
            If sInput.Length >= 162 Then                                                ' Is the input the right size?
                Dim uCells(9, 9) As CellStateClass                                      ' Yes, dim a grid
                Dim iPtr As Int32 = 0
                For iCol As Int32 = 1 To 9                                              ' Loop through each cell of the grid
                    For iRow As Int32 = 1 To 9
                        Dim sTemp As String = sInput.Substring(iPtr, 2)                 ' Grab a substring 
                        uCells(iCol, iRow) = New CellStateClass(iCol, iRow, sTemp)      ' Process the substring
                        If uCells(iCol, iRow).InvalidState Then                         ' Cell processed okay?
                            Return Nothing                                              ' No, return nothing
                        End If
                        iPtr += 2                                                       ' Increment the pointer by 2
                    Next
                Next
                Return uCells                                                           ' Return the grid
            Else
                Return Nothing                                                          ' Input too short, return nothing
            End If
        End Function

        Private Sub UpdateCount(iCount As Int32)
            Dim e As New GameManagerEventArgs(_eLevel, iCount)
            RaiseEvent GameManagerEvent(Me, e)
        End Sub

#End Region

#End Region

    End Class

End Namespace