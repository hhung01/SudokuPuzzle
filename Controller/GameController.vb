'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

' TODO: Save current board to My.Settings so that the next time the game
'       starts, we can load it and the user can continue where they
'       left off.
'
' TODO: When game is paused, doodle something on the panel.
'
' TODO: Keep track of the last 10 best times per level
'
' TODO: Allow user to change the game colors.  Or maybe create themes that the user can switch to.
'
' TODO: Employ more advanced puzzle ranking algorithms.

Imports System.Threading
Imports System.Text
Imports SudokuPuzzle.Model
Imports SudokuPuzzle.SharedFunctions
Imports SudokuPuzzle.Controller.GameGenerator

Namespace Controller

    Friend Class GameController
        '
        ' This class controls the game and manages all aspects of the game behind
        ' the UI.  Basically the "C" in the MVC programming model.  The UI calls
        ' methods and properties on this class.  And this class communicates 
        ' asynchronously with the UI using Events.  All the "business" logic
        ' to run the game goes in here.

        ' Let's establish some basics about the game.  We'll use Excel as a template
        ' to establish naming conventions.  The Classic Sudoku board is basically a
        ' 9x9 grid, table, or board.  But other grids can be 4x4 or 16x16 or even 
        ' larger.  But let's keep things simple and stick with a 9x9 grid.  Maybe 
        ' in the future I'll modify the game so that users can select between 
        ' different sizes.   The 9x9 grid is broken down into 3x3 boxes or mini-grids.
        ' On a 9x9 board, there are 3 mini-grids across and 3 mini-grids going down.
        '
        ' Sudoku has one simple rule: the numbers 1 through 9 appear in each row, 
        ' column, and 3x3 box only once.  The game starts with several blank cells
        ' and the object of the game is to fill in numbers in the proper cells so
        ' that the rule is followed.
        '
        ' We'll use the following naming conventions:
        '
        '   Grid = the 9x9 playing board of the game.
        '
        '   Region = the 3x3 box or mini-grid within the main grid
        '
        '   Cell = each element of the grid.
        ' 
        ' For reference, here is a diagram showing how each cell is referenced in code.
        ' Basically, it's by [col][row] much like how Excel worksheets do it.  Sure
        ' we can use zero through 8 since all arrays in .Net are zero based.  But we'll
        ' use 1 through 9 instead just to keep things simple.
        '
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
        ' Here is how we will number each region.
        '
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
        ' Here is another diagram showing how the 9x9 grid is indexed as 
        ' a single dimensional array from 0 to 80 (for a zero based array)
        '
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
        '

        ' TODO: Expand the game to accommodate 4x4 and 16x16 games.

#Region " Variables, Constants, And other Declarations "

#Region " Constants "

        ' This should match the GameLevelEnum
        Private Const _cMaxLevels As Int32 = 4

#End Region

#Region " Variables "

        Private _View As frmMain
        Private _Model As GameModel
        Private _eStartButtonState As StartButtonStateEnum

#End Region

#Region " Variables With Events "

        Private WithEvents _clsGameTimer As GameTimer
        Private WithEvents _clsGamesManager As GamesManager

#End Region

#Region " Other Declarations "

        Friend Delegate Sub GameControllerEventHandler(sender As Object, e As GameControllerEventArgs)
        Friend Event GameControllerEvent As GameControllerEventHandler

#End Region

#End Region

#Region " Private Properties "

        Private Property CurCell As CellIndex
        Private Property PrevCell As CellIndex
        Private Property GameInProgress As Boolean
        Private Property PuzzleComplete As Boolean
        Private Property StartButtonState As StartButtonStateEnum
            Get
                Return _eStartButtonState                           ' Return the start button state
            End Get
            Set(value As StartButtonStateEnum)
                _eStartButtonState = value                          ' Save the game state
                _View.SetStartButtonText = value                    ' Set the start button text
            End Set
        End Property

#End Region

#Region " Constructors "

        Friend Sub New(view As Form)
            _View = CType(view, frmMain)                            ' Save the View variable
            InitControllerVariables()                               ' Init any controller variables
        End Sub

#End Region

#Region " Event Handlers "

        Private Sub _GameTimer_GameTimerEvent(sender As Object, e As GameTimerEventArgs) Handles _clsGameTimer.GameTimerEvent
            RaiseGameControllerEvent(e.ElapsedTime)                 ' Pass the timer event to the View.
        End Sub

        Private Sub _clsGamesManager_GameManagerEvent(sender As Object, e As GameManagerEventArgs) Handles _clsGamesManager.GamesManagerEvent
            RaiseGameControllerEvent(e.Level, e.Value)              ' Pass the game level and game count to the View.
        End Sub

#End Region

#Region " Methods "

#Region " Public Methods "

#Region " Public Methods: UI Events "

        Friend Sub FormClickedEvent()
            FormClicked()
        End Sub

        Friend Sub FormClosing()
            StopGame()
            SaveSettings()
        End Sub

        Friend Sub CloseButtonClicked()
            _clsGamesManager.StopGamesManager()
        End Sub

        Friend Sub NewButtonClicked()
            LoadNewGame()
        End Sub

        Friend Sub StartButtonClicked()
            StartGame()
        End Sub

        Friend Sub ResetButtonClicked()
            ResetGame()
        End Sub

        Friend Sub HintButtonClicked()
            ShowHint()
        End Sub

        Friend Sub ClearButtonClicked()
            ClearCell()
        End Sub

        Friend Sub PrintButtonClicked()
            ' TODO: implement print routine
        End Sub

        Friend Sub HelpButtonClicked()
            ShowHelp()
        End Sub

        Friend Sub ShowSolutionCheckBoxClicked(bShowSolution As Boolean)
            ShowHideSolution(bShowSolution)
        End Sub

        Friend Sub ShowHideNotesCheckBoxClicked(bShowNotes As Boolean)
            ShowHideNotes(bShowNotes)
        End Sub

        Friend Sub NumberButtonClicked(btnIndex As Int32)
            ProcessNumberButton(btnIndex)
        End Sub

        Friend Sub CellClickedEvent(iCol As Int32, iRow As Int32)
            CellClicked(iCol, iRow)
        End Sub

        Friend Sub PaintCellEvent(iCol As Int32, iRow As Int32, e As PaintEventArgs)
            PaintCell(iCol, iRow, e)
        End Sub

#End Region

#Region " Public Methods: Form/Game Actions "

        Friend Sub ClearForm()
            _View.ClearAllCells()                               ' Clear all label text
            _View.EnterNotes = False                            ' Clear the checkboxes as the bottom
            _View.ShowAllNotes = False
            _View.ShowSolution = False
            _View.SetStatus = ""                                ' Clear both status displays
            _View.SetStatusBar = ""
        End Sub

        Friend Sub LoadSettings()
            _View.GameLevel = My.Settings.Level                 ' Set the drop down list on the view to the last level selected
            ' TODO: load a previous game if player is still playing
            ' Ask player if they want to load old game or new game
        End Sub

        Friend Sub LoadGameCounts()
            For I As Int32 = 0 To _cMaxLevels
                Dim eLevel As GameLevelEnum = CType(I, GameLevelEnum)               ' Convert counter to GameLevelEnum
                _View.GameLevelCount(eLevel) = _clsGamesManager.GameCount(eLevel)   ' Display the game counter in the View
            Next
        End Sub

#End Region

#End Region

#Region " Private Methods "

        Private Sub InitControllerVariables()
            _clsGameTimer = New GameTimer                               ' Initialize some of the internal variables
            _clsGamesManager = New GamesManager
            PuzzleComplete = False                                      ' Set flags to False
            GameInProgress = False
            StartButtonState = StartButtonStateEnum.StartGame           ' Set the start button state
        End Sub

        Private Sub RaiseGameControllerEvent(sElapsedTime As String)
            Dim e As New GameControllerEventArgs(sElapsedTime)          ' Create an EventArg
            RaiseEvent GameControllerEvent(Me, e)                       ' Raise the event
        End Sub

        Private Sub RaiseGameControllerEvent(eLevel As GameLevelEnum, value As Int32)
            Dim e As New GameControllerEventArgs(eLevel, value)         ' Create an EventArg
            RaiseEvent GameControllerEvent(Me, e)                       ' Raise the event
        End Sub

        Private Sub SaveSettings()
            My.Settings.Level = _View.GameLevel                         ' Save the last level played
            ' TODO: If game is still going, save the current state
        End Sub

        Private Sub ShowHelp()
            _View.ShowHelp()                                            ' Show help screen
        End Sub

        Private Sub LoadNewGame()
            If GameInProgress Then                                  ' Is there a game in progress?
                Dim iResult As MsgBoxResult = MsgBox("There is already a game in progress.  Do you want to play a new game?", MsgBoxStyle.YesNo, "Sudoku")
                If iResult = MsgBoxResult.Yes Then                  ' User really wants to start a new game?
                    GameEnded(False)                                ' Okay, reset some stuff
                    GetNewGame()                                    ' Get a new game
                End If
            Else                                                    ' There is no game in progress
                _View.EnableGameButtons(False, True)                ' Disable the view buttons
                _View.SetStatusBar = ""                             ' Clear the status bar
                _View.SetStatus = ""                                ' Clear elapsed time as well
                GetNewGame()                                        ' Load a new game
            End If
        End Sub

        Private Sub GetNewGame()
            If _clsGamesManager.GameCount(_View.LevelSelected) > 0 Then                 ' Any games available?
                _Model = New GameModel(_clsGamesManager.GetGame(_View.LevelSelected))   ' Yes, get it and load the Model
                _View.SetStatusBar = "New game loaded."                                 ' Display a message
                StartButtonState = StartButtonStateEnum.StartGame                       ' Set the start button state to "Start Game"
            Else                                                                        ' No games available ... tell user
                _View.SetStatusBar = "No games available for the selected level.  Please select another level."
            End If
        End Sub

        Private Sub StartGame()
            If GameInProgress Then                                          ' Game already in progress?
                If StartButtonState = StartButtonStateEnum.PauseGame Then   ' Yes, is the start button state "Pause"?
                    PauseGame()                                             ' Yes, then pause the game
                Else
                    ResumeGame()                                            ' No, must be resume ... so resume the game
                End If
            Else
                StartNewGame()                                              ' No game, so start a new game
            End If
        End Sub

        Private Sub StartNewGame()
            GameInProgress = True                                       ' Raise the GameInProgress flag
            ShowBoard()                                                 ' Show the grid
            _clsGameTimer.StartTimer()                                  ' Start the timer
            StartButtonState = StartButtonStateEnum.PauseGame           ' Set the start button state to "Pause"
            _View.EnableGameButtons(True, False)                        ' Enable the game buttons plus hide the panel blocking the grid
            UpdateStatusBarCount()                                      ' Display the empty count on screen for the first time
        End Sub

        Private Sub PauseGame()
            StartButtonState = StartButtonStateEnum.ResumeGame          ' Set start button state to "Resume"
            _clsGameTimer.PauseTimer()                                  ' Pause the timer
            _View.EnableGameButtons(False, True)                        ' Disable the game buttons and hide the game grid
        End Sub

        Private Sub ResumeGame()
            StartButtonState = StartButtonStateEnum.PauseGame           ' Set the start button state to "Pause"
            _clsGameTimer.ResumeTimer()                                 ' Resume the game timer
            _View.EnableGameButtons(True, False)                        ' Enable the game buttons and hide the panel blocking the game
        End Sub

        Private Sub GameEnded(bShowDialog As Boolean)
            _clsGameTimer.StopTimer()                                       ' Stop game timer
            PuzzleComplete = True                                           ' Raise PuzzleComplete flag
            GameInProgress = False                                          ' Clear GameInProgress flag
            If bShowDialog Then                                             ' Do we show the GameComplete dialog?
                _View.EnableGameButtons(False, False)                       ' Yes, disable the game button, but don't hide the game
                StartButtonState = StartButtonStateEnum.DisableButton       ' Set start button state to "Disable"
                _View.SetStatusBar = "Puzzle completed!"                    ' Set status bar text
                Thread.Sleep(0)                                             ' Sleep just to allow other threads to run
                _View.DisplayGameCompletedForm(_clsGameTimer.ElapsedTime)   ' Pop up the Game Complete dialog
            Else                                                            ' Don't show dialog
                _View.EnableGameButtons(False, True)                        ' Disable game buttons and hide the game
                StartButtonState = StartButtonStateEnum.StartGame           ' Set start button state to "Start"
            End If
        End Sub

        Private Sub ShowBoard()
            ClearForm()                                 ' Clear the game grid
            _View.InvalidateAllLabels()                 ' Repaint the board with the new data
            PuzzleComplete = False                      ' Clear the PuzzleComplete flag
        End Sub

        Private Sub PaintCell(iCol As Int32, iRow As Int32, e As PaintEventArgs)
            If Common.IsValidIndex(iCol, iRow) Then                                     ' Are indices valid?
                FillCellState(iCol, iRow, e)                                            ' Yes, fill Cell based on the state
                Dim SelectedCell As New CellIndex(iCol, iRow)
                If SelectedCell.IsSameCell(CurCell) Then                                ' Is the current cell pointer same as this cell?
                    If _Model.Cell(iCol, iRow).CellState <> CellStateEnum.Answer Then   ' Yes, is the cell an "Answer" cell?
                        HighlightCell(SelectedCell, e)                                  ' No, then highlight it
                    End If
                Else
                    UnHighlightCell(SelectedCell, e)                                    ' Otherwise, undo the highlight
                End If
            End If
        End Sub

        Private Sub FillCellState(iCol As Int32, iRow As Int32, e As PaintEventArgs)
            If _Model IsNot Nothing Then
                With _Model.Cell(iCol, iRow)
                    Select Case .CellState
                        Case CellStateEnum.Blank                                            ' Cell state = blank?
                            _View.ClearCell(iCol, iRow, e)                                  ' Yes, the clear the cell contents

                        Case CellStateEnum.Hint                                             ' Cell state = Hint?
                            _View.SetLabel(iCol, iRow, Color.BlueViolet, .Answer.ToString)  ' Yes, write the answer in purple


                        Case CellStateEnum.Notes                                            ' Cell state = Notes?
                            _View.ClearCell(iCol, iRow, e)                                  ' Yes, the clear the cell contents
                            DrawNotes(iCol, iRow, e)                                        ' Draw notes directly on the label

                        Case CellStateEnum.Answer                                           ' Cell state = Answer?
                            _View.SetLabel(iCol, iRow, Color.Black, .Answer.ToString)       ' Yes, write the answer in black

                        Case CellStateEnum.UserInput                                        ' Cell state = user answer?
                            Dim uColor As Color = Color.DarkRed                             ' Default color to dark red
                            If .IsCorrect Then                                              ' Is the user's answer correct?
                                uColor = Color.DarkGreen                                    ' Yes, change color to dark Green
                            End If
                            _View.SetLabel(iCol, iRow, uColor, .UserAnswer.ToString)        ' Display user's answer

                    End Select
                End With
            End If
        End Sub

        Private Sub DrawNotes(iCol As Int32, iRow As Int32, e As PaintEventArgs)
            With _Model.Cell(iCol, iRow)
                If .HasNotes Then                                               ' Does this cell have any notes?
                    _View.DrawNotes(iCol, iRow, e, .Notes)                      ' Yes, tell the View to draw them
                End If
            End With
        End Sub

        Private Sub ShowHideNotes(bShowNotes As Boolean)
            If bShowNotes Then                                                  ' Is the ShowNotes flag on?
                ShowNotes()                                                     ' Yes, then show the notes
            Else
                HideNotes()                                                     ' No, hide the notes
            End If
        End Sub

        Private Sub ShowNotes()
            For I As Int32 = 0 To _Model.Cells.Count - 1        ' Loop through all the cells
                With _Model.Cells(I)
                    If .CellState = CellStateEnum.Blank Then    ' Is the cell state blank?
                        .CellState = CellStateEnum.Notes        ' Yes, then change the state to Notes
                        _View.InvalidateLabel(.Col, .Row)       ' Force cell to repaint
                    End If
                End With
            Next
        End Sub

        Private Sub HideNotes()
            For I As Int32 = 0 To _Model.Cells.Count - 1        ' Loop through all the cells
                With _Model.Cells(I)
                    If .CellState = CellStateEnum.Notes Then    ' Is the cell state Notes?
                        .CellState = CellStateEnum.Blank        ' Yes, then change the state to Blank
                        _View.InvalidateLabel(.Col, .Row)       ' Force cell to repaint
                    End If
                End With
            Next
        End Sub

        Private Sub ShowHideSolution(bShowSolution As Boolean)
            If bShowSolution Then                               ' Show solution?
                ShowSolution()                                  ' Yes, the show it
            Else
                HideSolution()                                  ' No, hide it
            End If
        End Sub

        Private Sub ShowSolution()
            For I As Int32 = 0 To _Model.Cells.Count - 1        ' Loop through all the cells
                With _Model.Cells(I)
                    If (.CellState <> CellStateEnum.Answer) AndAlso (.CellState <> CellStateEnum.UserInput) Then
                        .CellState = CellStateEnum.Hint         ' Change the cell state flag to Hint
                        _View.InvalidateLabel(.Col, .Row)       ' Force cell to repaint
                    End If
                End With
            Next
        End Sub

        Private Sub HideSolution()
            For I As Int32 = 0 To _Model.Cells.Count - 1        ' Loop through all the cells
                With _Model.Cells(I)
                    If .CellState = CellStateEnum.Hint Then     ' If the cell state is "Hint"
                        If _View.ShowAllNotes Then              ' Yes, is the show all notes checkbox checked?
                            .CellState = CellStateEnum.Notes    ' Yes, set the state to "Notes"
                        Else
                            .CellState = CellStateEnum.Blank    ' No, set the state to "Blank"
                        End If
                        _View.InvalidateLabel(.Col, .Row)       ' Force cell to repaint
                    End If
                End With
            Next
        End Sub

        Private Sub FormClicked()
            CurCell = Nothing                       ' User clicked outside the board.  Set current cell to nothing.
            UnHighlightPreviousCell()               ' Unhighlight the previous cell.
        End Sub

        Private Sub CellClicked(iCol As Int32, iRow As Int32)
            CurCell = New CellIndex(iCol, iRow)                                             ' Save the current cell pointer
            UnHighlightPreviousCell()                                                       ' Unhighlight previous cell
            With _Model.Cell(CurCell)
                _View.InvalidateLabel(iCol, iRow)                                           ' Force cell to repaint
                If (Not PuzzleComplete) AndAlso (.CellState <> CellStateEnum.Answer) Then   ' If the puzzle is not complete and cell state is not the answer
                    Dim iResult As Int32 = _View.ShowNumberPanel(iCol, iRow)                ' Show the mini input window
                    ProcessNumberButton(iResult)                                            ' Process the results
                End If
            End With
        End Sub

        Private Sub UnHighlightPreviousCell()
            If PrevCell IsNot Nothing Then                              ' Any previous cell selected?
                _View.InvalidateLabel(PrevCell.Col, PrevCell.Row)       ' Yes, force the cell to repaint
            End If
        End Sub

        Private Sub HighlightCell(uSelectedCell As CellIndex, e As PaintEventArgs)
            With uSelectedCell
                _View.DrawBorder(.Col, .Row, e, True)                   ' Draw a border to highlight the cell
            End With
            PrevCell = uSelectedCell                                    ' Save selected cell to previous pointer
        End Sub

        Private Sub UnHighlightCell(uSelectedCell As CellIndex, e As PaintEventArgs)
            With uSelectedCell
                _View.DrawBorder(.Col, .Row, e, False)                  ' Delete border around the cell
            End With
        End Sub

        Private Sub ProcessNumberButton(value As Int32)
            If Common.IsValidIndex(value) AndAlso CurCell IsNot Nothing Then    ' Is the input number valid and the current cell pointer is valid?
                ProcessNumberButton(_Model.Cell(CurCell), value)                ' Yes, then process it
            End If
        End Sub

        Private Sub ProcessNumberButton(uCell As CellStateClass, value As Int32)
            If uCell.CellState <> CellStateEnum.Answer Then                 ' Is cell state not the answer?
                If _View.EnterNotes Then                                    ' Yes, user wants to enter notes?
                    ProcessUserNotes(uCell, value)                          ' Yes, process notes
                Else                                                        ' No, user is attempting to enter a number
                    ProcessUserAnswer(uCell, value)                         ' Process user's input
                End If
            End If
        End Sub

        Private Sub ProcessUserNotes(uCell As CellStateClass, value As Int32)
            With uCell
                Select Case .CellState                                  ' Check the Cell state
                    Case CellStateEnum.Blank                            ' Is it blank?
                        .ClearNotes()                                   ' Yes, clear any previous notes
                        .CellState = CellStateEnum.Notes                ' Set cell state to notes
                        .Notes(value) = Not .Notes(value)               ' Raise the corresponding note index

                    Case CellStateEnum.Notes                            ' Already in Note mode?
                        .Notes(value) = Not .Notes(value)               ' Yes, then toggle the corresponding note index

                End Select
                _View.InvalidateLabel(.Col, .Row)                       ' Force cell to repaint
            End With
        End Sub

        Private Sub ProcessUserAnswer(uCell As CellStateClass, value As Int32)
            With uCell
                .UserAnswer = value                             ' Save user's answer
                .CellState = CellStateEnum.UserInput            ' Set cell state to user answer
                _View.InvalidateLabel(.Col, .Row)               ' Force cell to repaint
                If .IsCorrect Then                              ' Is it correct?
                    ScanNotes(CurCell, value)                   ' Yes, turn off notes in surrounding cells
                    UpdateStatusBarCount(-1)                    ' Decrement and update screen count
                End If
            End With
        End Sub

        Private Sub UpdateStatusBarCount(Optional iCount As Int32 = 0)
            _Model.EmptyCount += iCount                         ' Increment or decrement the Empty count
            _View.SetStatusBar = _Model.EmptyCount.ToString & " empty cells out of 81."
            If _Model.GameComplete Then                         ' Are all the blank cells filled in?
                GameEnded(True)                                 ' Yes, then go to end game routine
            End If
        End Sub

        Private Sub ScanNotes(uIndex As CellIndex, value As Int32)
            With uIndex
                For iScan As Int32 = 1 To 9
                    CheckNotes(_Model.Cell(iScan, .Row), value)                     ' Scan the column
                    CheckNotes(_Model.Cell(.Col, iScan), value)                     ' Scan the rows
                Next
                Dim uList As List(Of CellStateClass) = _Model.RegionCells(.Region)  ' Grab all the cells in this region
                If uList IsNot Nothing Then                                         ' Check to make sure that list is not empty
                    For I As Int32 = 0 To 8                                         ' Loop through the list
                        If uList(I) IsNot Nothing Then                              ' Check to make sure element is not empty
                            CheckNotes(uList(I), value)                             ' Check the notes
                        End If
                    Next
                End If
            End With
        End Sub

        Private Sub CheckNotes(uCell As CellStateClass, value As Int32)
            With uCell
                If .CellState = CellStateEnum.Notes AndAlso .Notes(value) Then  ' Is cell in note mode and corresponding note flag is raised?
                    .Notes(value) = False                                       ' Yes, then turn it off
                    _View.InvalidateLabel(.Col, .Row)                           ' Force cell to repaint
                End If
            End With
        End Sub

        Private Sub ClearCell()
            If CurCell IsNot Nothing Then                                   ' Current cell pointer valid?
                With _Model.Cell(CurCell)                                   ' Yes, then process it
                    If .CellState <> CellStateEnum.Answer Then              ' Are we displaying the answer?
                        If UndoEmptyCount(_Model.Cell(CurCell)) Then        ' No, check if we need to increment the empty counter
                            UpdateStatusBarCount(1)                         ' Yes, increment and update screen count
                        End If
                        .CellState = CellStateEnum.Blank                    ' Set cell state back to "Blank"
                        .UserAnswer = 0                                     ' Clear user's answer
                        _Model.ComputeNote(CurCell.Col, CurCell.Row)        ' Recompute note for this cell
                        _View.InvalidateLabel(CurCell)                      ' Force cell to repaint
                    End If
                End With
            End If
        End Sub

        Private Function UndoEmptyCount(uCell As CellStateClass) As Boolean
            With uCell
                ' Return true if cell state = Hint or cell state = user answer and it's correct
                If (.CellState = CellStateEnum.Hint) OrElse _
                    ((.CellState = CellStateEnum.UserInput) AndAlso (.IsCorrect)) Then
                    Return True
                End If
            End With
            Return False                                                    ' Return False if condition failed
        End Function

        Private Sub ShowHint()
            If CurCell IsNot Nothing Then                                   ' Current cell pointer valid?
                With _Model.Cell(CurCell)                                   ' Yes, then process it
                    If .CellState <> CellStateEnum.Answer Then              ' Are we displaying the answer?
                        .CellState = CellStateEnum.Hint                     ' No, then set it to Hint
                        _View.InvalidateLabel(CurCell)                      ' Force cell to repaint
                        UpdateStatusBarCount(-1)                            ' Decrement and update the screen count
                    End If
                End With
            End If
        End Sub

        Private Sub ResetGame()
            ClearForm()                                             ' Clear game grid
            _clsGameTimer.ResetTimer()                              ' Restart the timer
            For I As Int32 = 0 To _Model.Cells.Count - 1            ' Loop through all the cells
                With _Model.Cells(I)
                    If .CellState <> CellStateEnum.Answer Then      ' If not displaying the answer
                        .CellState = CellStateEnum.Blank            ' Set state to blank
                        .UserAnswer = 0                             ' Clear out any previous answer
                        _View.InvalidateLabel(.Col, .Row)           ' Invalidate the cell
                    End If
                End With
            Next
        End Sub

        Private Sub StopGame()
            If GameInProgress Then                                ' If game is in progress ...?
                ' Do we need to do something here?
            End If
        End Sub

#End Region

#End Region

    End Class

End Namespace
