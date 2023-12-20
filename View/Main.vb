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
Imports SudokuPuzzle.Controller
Imports SudokuPuzzle.SharedFunctions

Public Class frmMain

#Region " Variables, Constants, And other Declarations "

#Region " Variables "

    Private _bInitComplete As Boolean = False
    Private _uLabels(9, 9) As Label

#End Region

#Region " Variables WithEvents "

    Private WithEvents _clsGameController As GameController

#End Region

#Region " Other Declarations "

    Private Delegate Sub SetStatusCallback(sMsg As String)
    Private Delegate Sub SetStatusBarCallback(sMsg As String)
    Private Delegate Sub SetGameCountCallback(lLabel As ToolStripLabel, value As Int32)

#End Region

#End Region

#Region " Public Properties "

#Region " Public Properties: Read/Write Properties "

    Friend Property EnterNotes As Boolean
        Get
            Return ckbEnterNotes.Checked
        End Get
        Set(value As Boolean)
            ckbEnterNotes.Checked = value
        End Set
    End Property

    Friend Property ShowAllNotes As Boolean
        Get
            Return ckbShowAllNotes.Checked
        End Get
        Set(value As Boolean)
            ckbShowAllNotes.Checked = value
        End Set
    End Property

    Friend Property ShowSolution As Boolean
        Get
            Return ckbShowSolution.Checked
        End Get
        Set(value As Boolean)
            ckbShowSolution.Checked = value
        End Set
    End Property

    Friend Property GameLevel As Int32
        Get
            Return cbDifficultyLevel.SelectedIndex
        End Get
        Set(value As Int32)
            If _bInitComplete Then
                If SharedFunctions.Common.IsValidStateEnum(value) Then
                    cbDifficultyLevel.SelectedIndex = value
                End If
            End If
        End Set
    End Property

#End Region

#Region " Public: Readonly Properties "

    Friend ReadOnly Property LevelSelected As GameLevelEnum
        Get
            Return CType(cbDifficultyLevel.SelectedIndex, GameLevelEnum)
        End Get
    End Property

#End Region

#Region " Public Properties: Write Only Properties "

    Friend WriteOnly Property SetStartButtonText As StartButtonStateEnum
        Set(value As StartButtonStateEnum)
            If value = StartButtonStateEnum.DisableButton Then      ' Disable the start button?
                btnStart.Enabled = False                            ' Yes, disable it
            Else
                btnStart.Enabled = True                             ' No, enable it
                Select Case value
                    Case StartButtonStateEnum.PauseGame             ' State = Pause game?
                        btnStart.Text = "Pause Game"                ' Yes, set button text to "Pause Game"

                    Case StartButtonStateEnum.ResumeGame            ' State = Resme game?
                        btnStart.Text = "Resume Game"               ' Yes, set text to "Resume"

                    Case Else
                        btnStart.Text = "Start Game"                ' No, then set to "Start"

                End Select
            End If
        End Set
    End Property

    Friend WriteOnly Property SetStatusBar As String
        Set(value As String)
            SetStatusBarText(value)
        End Set
    End Property

    Friend WriteOnly Property SetStatus As String
        Set(value As String)
            SetStatusText(value)
        End Set
    End Property

    Friend WriteOnly Property GameLevelCount(eLevel As GameLevelEnum) As Int32
        Set(value As Int32)
            SetGameCount(eLevel, value)
        End Set
    End Property


#End Region

#End Region

#Region " Event Handlers "

#Region " Event Handlers: Form "

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartGameController()                   ' Start the game controller
        InitForm()                              ' Initialize the forms and any variables
        ClearForm()                             ' Clear the board
        EnableGameButtons(False, True)          ' Disable the game buttons and show the panel
        _bInitComplete = True                   ' Raise init complete flag
        _clsGameController.LoadSettings()       ' Load settings from the controller
        _clsGameController.LoadGameCounts()     ' Load the game counts from the controller
    End Sub

    Private Sub frmMain_Click(sender As Object, e As EventArgs) Handles Me.Click
        _clsGameController.FormClickedEvent()
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        _clsGameController.FormClosing()
    End Sub

    Private Sub cbDifficultyLevel_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDifficultyLevel.SelectedIndexChanged
        If _bInitComplete Then
            _clsGameController.NewButtonClicked()
        End If
    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        _clsGameController.NewButtonClicked()
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        _clsGameController.StartButtonClicked()
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        _clsGameController.ResetButtonClicked()
    End Sub

    Private Sub btnHint_Click(sender As Object, e As EventArgs) Handles btnHint.Click
        _clsGameController.HintButtonClicked()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        _clsGameController.ClearButtonClicked()
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        _clsGameController.PrintButtonClicked()
    End Sub

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        _clsGameController.HelpButtonClicked()
    End Sub

    Private Sub ckbShowAllNotes_CheckedChanged(sender As Object, e As EventArgs) Handles ckbShowAllNotes.CheckedChanged
        _clsGameController.ShowHideNotesCheckBoxClicked(ckbShowAllNotes.Checked)
    End Sub

    Private Sub ckbShowSolution_CheckedChanged(sender As Object, e As EventArgs) Handles ckbShowSolution.CheckedChanged
        _clsGameController.ShowSolutionCheckBoxClicked(ckbShowSolution.Checked)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        _clsGameController.CloseButtonClicked()
        Me.Close()
    End Sub

#End Region

#Region " Event Handlers: Cell Labels "

    Private Sub LabelA1_Click(sender As Object, e As EventArgs) Handles LabelA1.Click
        _clsGameController.CellClickedEvent(1, 1)
    End Sub

    Private Sub LabelA1_Paint(sender As Object, e As PaintEventArgs) Handles LabelA1.Paint
        _clsGameController.PaintCellEvent(1, 1, e)
    End Sub

    Private Sub LabelB1_Click(sender As Object, e As EventArgs) Handles LabelB1.Click
        _clsGameController.CellClickedEvent(2, 1)
    End Sub

    Private Sub LabelB1_Paint(sender As Object, e As PaintEventArgs) Handles LabelB1.Paint
        _clsGameController.PaintCellEvent(2, 1, e)
    End Sub

    Private Sub LabelC1_Click(sender As Object, e As EventArgs) Handles LabelC1.Click
        _clsGameController.CellClickedEvent(3, 1)
    End Sub

    Private Sub LabelC1_Paint(sender As Object, e As PaintEventArgs) Handles LabelC1.Paint
        _clsGameController.PaintCellEvent(3, 1, e)
    End Sub

    Private Sub LabelD1_Click(sender As Object, e As EventArgs) Handles LabelD1.Click
        _clsGameController.CellClickedEvent(4, 1)
    End Sub

    Private Sub LabelD1_Paint(sender As Object, e As PaintEventArgs) Handles LabelD1.Paint
        _clsGameController.PaintCellEvent(4, 1, e)
    End Sub

    Private Sub LabelE1_Click(sender As Object, e As EventArgs) Handles LabelE1.Click
        _clsGameController.CellClickedEvent(5, 1)
    End Sub

    Private Sub LabelE1_Paint(sender As Object, e As PaintEventArgs) Handles LabelE1.Paint
        _clsGameController.PaintCellEvent(5, 1, e)
    End Sub

    Private Sub LabelF1_Click(sender As Object, e As EventArgs) Handles LabelF1.Click
        _clsGameController.CellClickedEvent(6, 1)
    End Sub

    Private Sub LabelF1_Paint(sender As Object, e As PaintEventArgs) Handles LabelF1.Paint
        _clsGameController.PaintCellEvent(6, 1, e)
    End Sub

    Private Sub LabelG1_Click(sender As Object, e As EventArgs) Handles LabelG1.Click
        _clsGameController.CellClickedEvent(7, 1)
    End Sub

    Private Sub LabelG1_Paint(sender As Object, e As PaintEventArgs) Handles LabelG1.Paint
        _clsGameController.PaintCellEvent(7, 1, e)
    End Sub

    Private Sub LabelH1_Click(sender As Object, e As EventArgs) Handles LabelH1.Click
        _clsGameController.CellClickedEvent(8, 1)
    End Sub

    Private Sub LabelH1_Paint(sender As Object, e As PaintEventArgs) Handles LabelH1.Paint
        _clsGameController.PaintCellEvent(8, 1, e)
    End Sub

    Private Sub LabelI1_Click(sender As Object, e As EventArgs) Handles LabelI1.Click
        _clsGameController.CellClickedEvent(9, 1)
    End Sub

    Private Sub LabelI1_Paint(sender As Object, e As PaintEventArgs) Handles LabelI1.Paint
        _clsGameController.PaintCellEvent(9, 1, e)
    End Sub

    Private Sub LabelA2_Click(sender As Object, e As EventArgs) Handles LabelA2.Click
        _clsGameController.CellClickedEvent(1, 2)
    End Sub

    Private Sub LabelA2_Paint(sender As Object, e As PaintEventArgs) Handles LabelA2.Paint
        _clsGameController.PaintCellEvent(1, 2, e)
    End Sub

    Private Sub LabelB2_Click(sender As Object, e As EventArgs) Handles LabelB2.Click
        _clsGameController.CellClickedEvent(2, 2)
    End Sub

    Private Sub LabelB2_Paint(sender As Object, e As PaintEventArgs) Handles LabelB2.Paint
        _clsGameController.PaintCellEvent(2, 2, e)
    End Sub

    Private Sub LabelC2_Click(sender As Object, e As EventArgs) Handles LabelC2.Click
        _clsGameController.CellClickedEvent(3, 2)
    End Sub

    Private Sub LabelC2_Paint(sender As Object, e As PaintEventArgs) Handles LabelC2.Paint
        _clsGameController.PaintCellEvent(3, 2, e)
    End Sub

    Private Sub LabelD2_Click(sender As Object, e As EventArgs) Handles LabelD2.Click
        _clsGameController.CellClickedEvent(4, 2)
    End Sub

    Private Sub LabelD2_Paint(sender As Object, e As PaintEventArgs) Handles LabelD2.Paint
        _clsGameController.PaintCellEvent(4, 2, e)
    End Sub

    Private Sub LabelE2_Click(sender As Object, e As EventArgs) Handles LabelE2.Click
        _clsGameController.CellClickedEvent(5, 2)
    End Sub

    Private Sub LabelE2_Paint(sender As Object, e As PaintEventArgs) Handles LabelE2.Paint
        _clsGameController.PaintCellEvent(5, 2, e)
    End Sub

    Private Sub LabelF2_Click(sender As Object, e As EventArgs) Handles LabelF2.Click
        _clsGameController.CellClickedEvent(6, 2)
    End Sub

    Private Sub LabelF2_Paint(sender As Object, e As PaintEventArgs) Handles LabelF2.Paint
        _clsGameController.PaintCellEvent(6, 2, e)
    End Sub

    Private Sub LabelG2_Click(sender As Object, e As EventArgs) Handles LabelG2.Click
        _clsGameController.CellClickedEvent(7, 2)
    End Sub

    Private Sub LabelG2_Paint(sender As Object, e As PaintEventArgs) Handles LabelG2.Paint
        _clsGameController.PaintCellEvent(7, 2, e)
    End Sub

    Private Sub LabelH2_Click(sender As Object, e As EventArgs) Handles LabelH2.Click
        _clsGameController.CellClickedEvent(8, 2)
    End Sub

    Private Sub LabelH2_Paint(sender As Object, e As PaintEventArgs) Handles LabelH2.Paint
        _clsGameController.PaintCellEvent(8, 2, e)
    End Sub

    Private Sub LabelI2_Click(sender As Object, e As EventArgs) Handles LabelI2.Click
        _clsGameController.CellClickedEvent(9, 2)
    End Sub

    Private Sub LabelI2_Paint(sender As Object, e As PaintEventArgs) Handles LabelI2.Paint
        _clsGameController.PaintCellEvent(9, 2, e)
    End Sub

    Private Sub LabelA3_Click(sender As Object, e As EventArgs) Handles LabelA3.Click
        _clsGameController.CellClickedEvent(1, 3)
    End Sub

    Private Sub LabelA3_Paint(sender As Object, e As PaintEventArgs) Handles LabelA3.Paint
        _clsGameController.PaintCellEvent(1, 3, e)
    End Sub

    Private Sub LabelB3_Click(sender As Object, e As EventArgs) Handles LabelB3.Click
        _clsGameController.CellClickedEvent(2, 3)
    End Sub

    Private Sub LabelB3_Paint(sender As Object, e As PaintEventArgs) Handles LabelB3.Paint
        _clsGameController.PaintCellEvent(2, 3, e)
    End Sub

    Private Sub LabelC3_Click(sender As Object, e As EventArgs) Handles LabelC3.Click
        _clsGameController.CellClickedEvent(3, 3)
    End Sub

    Private Sub LabelC3_Paint(sender As Object, e As PaintEventArgs) Handles LabelC3.Paint
        _clsGameController.PaintCellEvent(3, 3, e)
    End Sub

    Private Sub LabelD3_Click(sender As Object, e As EventArgs) Handles LabelD3.Click
        _clsGameController.CellClickedEvent(4, 3)
    End Sub

    Private Sub LabelD3_Paint(sender As Object, e As PaintEventArgs) Handles LabelD3.Paint
        _clsGameController.PaintCellEvent(4, 3, e)
    End Sub

    Private Sub LabelE3_Click(sender As Object, e As EventArgs) Handles LabelE3.Click
        _clsGameController.CellClickedEvent(5, 3)
    End Sub

    Private Sub LabelE3_Paint(sender As Object, e As PaintEventArgs) Handles LabelE3.Paint
        _clsGameController.PaintCellEvent(5, 3, e)
    End Sub

    Private Sub LabelF3_Click(sender As Object, e As EventArgs) Handles LabelF3.Click
        _clsGameController.CellClickedEvent(6, 3)
    End Sub

    Private Sub LabelF3_Paint(sender As Object, e As PaintEventArgs) Handles LabelF3.Paint
        _clsGameController.PaintCellEvent(6, 3, e)
    End Sub

    Private Sub LabelG3_Click(sender As Object, e As EventArgs) Handles LabelG3.Click
        _clsGameController.CellClickedEvent(7, 3)
    End Sub

    Private Sub LabelG3_Paint(sender As Object, e As PaintEventArgs) Handles LabelG3.Paint
        _clsGameController.PaintCellEvent(7, 3, e)
    End Sub

    Private Sub LabelH3_Click(sender As Object, e As EventArgs) Handles LabelH3.Click
        _clsGameController.CellClickedEvent(8, 3)
    End Sub

    Private Sub LabelH3_Paint(sender As Object, e As PaintEventArgs) Handles LabelH3.Paint
        _clsGameController.PaintCellEvent(8, 3, e)
    End Sub

    Private Sub LabelI3_Click(sender As Object, e As EventArgs) Handles LabelI3.Click
        _clsGameController.CellClickedEvent(9, 3)
    End Sub

    Private Sub LabelI3_Paint(sender As Object, e As PaintEventArgs) Handles LabelI3.Paint
        _clsGameController.PaintCellEvent(9, 3, e)
    End Sub

    Private Sub LabelA4_Click(sender As Object, e As EventArgs) Handles LabelA4.Click
        _clsGameController.CellClickedEvent(1, 4)
    End Sub

    Private Sub LabelA4_Paint(sender As Object, e As PaintEventArgs) Handles LabelA4.Paint
        _clsGameController.PaintCellEvent(1, 4, e)
    End Sub

    Private Sub LabelB4_Click(sender As Object, e As EventArgs) Handles LabelB4.Click
        _clsGameController.CellClickedEvent(2, 4)
    End Sub

    Private Sub LabelB4_Paint(sender As Object, e As PaintEventArgs) Handles LabelB4.Paint
        _clsGameController.PaintCellEvent(2, 4, e)
    End Sub

    Private Sub LabelC4_Click(sender As Object, e As EventArgs) Handles LabelC4.Click
        _clsGameController.CellClickedEvent(3, 4)
    End Sub

    Private Sub LabelC4_Paint(sender As Object, e As PaintEventArgs) Handles LabelC4.Paint
        _clsGameController.PaintCellEvent(3, 4, e)
    End Sub

    Private Sub LabelD4_Click(sender As Object, e As EventArgs) Handles LabelD4.Click
        _clsGameController.CellClickedEvent(4, 4)
    End Sub

    Private Sub LabelD4_Paint(sender As Object, e As PaintEventArgs) Handles LabelD4.Paint
        _clsGameController.PaintCellEvent(4, 4, e)
    End Sub

    Private Sub LabelE4_Click(sender As Object, e As EventArgs) Handles LabelE4.Click
        _clsGameController.CellClickedEvent(5, 4)
    End Sub

    Private Sub LabelE4_Paint(sender As Object, e As PaintEventArgs) Handles LabelE4.Paint
        _clsGameController.PaintCellEvent(5, 4, e)
    End Sub

    Private Sub LabelF4_Click(sender As Object, e As EventArgs) Handles LabelF4.Click
        _clsGameController.CellClickedEvent(6, 4)
    End Sub

    Private Sub LabelF4_Paint(sender As Object, e As PaintEventArgs) Handles LabelF4.Paint
        _clsGameController.PaintCellEvent(6, 4, e)
    End Sub

    Private Sub LabelG4_Click(sender As Object, e As EventArgs) Handles LabelG4.Click
        _clsGameController.CellClickedEvent(7, 4)
    End Sub

    Private Sub LabelG4_Paint(sender As Object, e As PaintEventArgs) Handles LabelG4.Paint
        _clsGameController.PaintCellEvent(7, 4, e)
    End Sub

    Private Sub LabelH4_Click(sender As Object, e As EventArgs) Handles LabelH4.Click
        _clsGameController.CellClickedEvent(8, 4)
    End Sub

    Private Sub LabelH4_Paint(sender As Object, e As PaintEventArgs) Handles LabelH4.Paint
        _clsGameController.PaintCellEvent(8, 4, e)
    End Sub

    Private Sub LabelI4_Click(sender As Object, e As EventArgs) Handles LabelI4.Click
        _clsGameController.CellClickedEvent(9, 4)
    End Sub

    Private Sub LabelI4_Paint(sender As Object, e As PaintEventArgs) Handles LabelI4.Paint
        _clsGameController.PaintCellEvent(9, 4, e)
    End Sub

    Private Sub LabelA5_Click(sender As Object, e As EventArgs) Handles LabelA5.Click
        _clsGameController.CellClickedEvent(1, 5)
    End Sub

    Private Sub LabelA5_Paint(sender As Object, e As PaintEventArgs) Handles LabelA5.Paint
        _clsGameController.PaintCellEvent(1, 5, e)
    End Sub

    Private Sub LabelB5_Click(sender As Object, e As EventArgs) Handles LabelB5.Click
        _clsGameController.CellClickedEvent(2, 5)
    End Sub

    Private Sub LabelB5_Paint(sender As Object, e As PaintEventArgs) Handles LabelB5.Paint
        _clsGameController.PaintCellEvent(2, 5, e)
    End Sub

    Private Sub LabelC5_Click(sender As Object, e As EventArgs) Handles LabelC5.Click
        _clsGameController.CellClickedEvent(3, 5)
    End Sub

    Private Sub LabelC5_Paint(sender As Object, e As PaintEventArgs) Handles LabelC5.Paint
        _clsGameController.PaintCellEvent(3, 5, e)
    End Sub

    Private Sub LabelD5_Click(sender As Object, e As EventArgs) Handles LabelD5.Click
        _clsGameController.CellClickedEvent(4, 5)
    End Sub

    Private Sub LabelD5_Paint(sender As Object, e As PaintEventArgs) Handles LabelD5.Paint
        _clsGameController.PaintCellEvent(4, 5, e)
    End Sub

    Private Sub LabelE5_Click(sender As Object, e As EventArgs) Handles LabelE5.Click
        _clsGameController.CellClickedEvent(5, 5)
    End Sub

    Private Sub LabelE5_Paint(sender As Object, e As PaintEventArgs) Handles LabelE5.Paint
        _clsGameController.PaintCellEvent(5, 5, e)
    End Sub

    Private Sub LabelF5_Click(sender As Object, e As EventArgs) Handles LabelF5.Click
        _clsGameController.CellClickedEvent(6, 5)
    End Sub

    Private Sub LabelF5_Paint(sender As Object, e As PaintEventArgs) Handles LabelF5.Paint
        _clsGameController.PaintCellEvent(6, 5, e)
    End Sub

    Private Sub LabelG5_Click(sender As Object, e As EventArgs) Handles LabelG5.Click
        _clsGameController.CellClickedEvent(7, 5)
    End Sub

    Private Sub LabelG5_Paint(sender As Object, e As PaintEventArgs) Handles LabelG5.Paint
        _clsGameController.PaintCellEvent(7, 5, e)
    End Sub

    Private Sub LabelH5_Click(sender As Object, e As EventArgs) Handles LabelH5.Click
        _clsGameController.CellClickedEvent(8, 5)
    End Sub

    Private Sub LabelH5_Paint(sender As Object, e As PaintEventArgs) Handles LabelH5.Paint
        _clsGameController.PaintCellEvent(8, 5, e)
    End Sub

    Private Sub LabelI5_Click(sender As Object, e As EventArgs) Handles LabelI5.Click
        _clsGameController.CellClickedEvent(9, 5)
    End Sub

    Private Sub LabelI5_Paint(sender As Object, e As PaintEventArgs) Handles LabelI5.Paint
        _clsGameController.PaintCellEvent(9, 5, e)
    End Sub

    Private Sub LabelA6_Click(sender As Object, e As EventArgs) Handles LabelA6.Click
        _clsGameController.CellClickedEvent(1, 6)
    End Sub

    Private Sub LabelA6_Paint(sender As Object, e As PaintEventArgs) Handles LabelA6.Paint
        _clsGameController.PaintCellEvent(1, 6, e)
    End Sub

    Private Sub LabelB6_Click(sender As Object, e As EventArgs) Handles LabelB6.Click
        _clsGameController.CellClickedEvent(2, 6)
    End Sub

    Private Sub LabelB6_Paint(sender As Object, e As PaintEventArgs) Handles LabelB6.Paint
        _clsGameController.PaintCellEvent(2, 6, e)
    End Sub

    Private Sub LabelC6_Click(sender As Object, e As EventArgs) Handles LabelC6.Click
        _clsGameController.CellClickedEvent(3, 6)
    End Sub

    Private Sub LabelC6_Paint(sender As Object, e As PaintEventArgs) Handles LabelC6.Paint
        _clsGameController.PaintCellEvent(3, 6, e)
    End Sub

    Private Sub LabelD6_Click(sender As Object, e As EventArgs) Handles LabelD6.Click
        _clsGameController.CellClickedEvent(4, 6)
    End Sub

    Private Sub LabelD6_Paint(sender As Object, e As PaintEventArgs) Handles LabelD6.Paint
        _clsGameController.PaintCellEvent(4, 6, e)
    End Sub

    Private Sub LabelE6_Click(sender As Object, e As EventArgs) Handles LabelE6.Click
        _clsGameController.CellClickedEvent(5, 6)
    End Sub

    Private Sub LabelE6_Paint(sender As Object, e As PaintEventArgs) Handles LabelE6.Paint
        _clsGameController.PaintCellEvent(5, 6, e)
    End Sub

    Private Sub LabelF6_Click(sender As Object, e As EventArgs) Handles LabelF6.Click
        _clsGameController.CellClickedEvent(6, 6)
    End Sub

    Private Sub LabelF6_Paint(sender As Object, e As PaintEventArgs) Handles LabelF6.Paint
        _clsGameController.PaintCellEvent(6, 6, e)
    End Sub

    Private Sub LabelG6_Click(sender As Object, e As EventArgs) Handles LabelG6.Click
        _clsGameController.CellClickedEvent(7, 6)
    End Sub

    Private Sub LabelG6_Paint(sender As Object, e As PaintEventArgs) Handles LabelG6.Paint
        _clsGameController.PaintCellEvent(7, 6, e)
    End Sub

    Private Sub LabelH6_Click(sender As Object, e As EventArgs) Handles LabelH6.Click
        _clsGameController.CellClickedEvent(8, 6)
    End Sub

    Private Sub LabelH6_Paint(sender As Object, e As PaintEventArgs) Handles LabelH6.Paint
        _clsGameController.PaintCellEvent(8, 6, e)
    End Sub

    Private Sub LabelI6_Click(sender As Object, e As EventArgs) Handles LabelI6.Click
        _clsGameController.CellClickedEvent(9, 6)
    End Sub

    Private Sub LabelI6_Paint(sender As Object, e As PaintEventArgs) Handles LabelI6.Paint
        _clsGameController.PaintCellEvent(9, 6, e)
    End Sub

    Private Sub LabelA7_Click(sender As Object, e As EventArgs) Handles LabelA7.Click
        _clsGameController.CellClickedEvent(1, 7)
    End Sub

    Private Sub LabelA7_Paint(sender As Object, e As PaintEventArgs) Handles LabelA7.Paint
        _clsGameController.PaintCellEvent(1, 7, e)
    End Sub

    Private Sub LabelB7_Click(sender As Object, e As EventArgs) Handles LabelB7.Click
        _clsGameController.CellClickedEvent(2, 7)
    End Sub

    Private Sub LabelB7_Paint(sender As Object, e As PaintEventArgs) Handles LabelB7.Paint
        _clsGameController.PaintCellEvent(2, 7, e)
    End Sub

    Private Sub LabelC7_Click(sender As Object, e As EventArgs) Handles LabelC7.Click
        _clsGameController.CellClickedEvent(3, 7)
    End Sub

    Private Sub LabelC7_Paint(sender As Object, e As PaintEventArgs) Handles LabelC7.Paint
        _clsGameController.PaintCellEvent(3, 7, e)
    End Sub

    Private Sub LabelD7_Click(sender As Object, e As EventArgs) Handles LabelD7.Click
        _clsGameController.CellClickedEvent(4, 7)
    End Sub

    Private Sub LabelD7_Paint(sender As Object, e As PaintEventArgs) Handles LabelD7.Paint
        _clsGameController.PaintCellEvent(4, 7, e)
    End Sub

    Private Sub LabelE7_Click(sender As Object, e As EventArgs) Handles LabelE7.Click
        _clsGameController.CellClickedEvent(5, 7)
    End Sub

    Private Sub LabelE7_Paint(sender As Object, e As PaintEventArgs) Handles LabelE7.Paint
        _clsGameController.PaintCellEvent(5, 7, e)
    End Sub

    Private Sub LabelF7_Click(sender As Object, e As EventArgs) Handles LabelF7.Click
        _clsGameController.CellClickedEvent(6, 7)
    End Sub

    Private Sub LabelF7_Paint(sender As Object, e As PaintEventArgs) Handles LabelF7.Paint
        _clsGameController.PaintCellEvent(6, 7, e)
    End Sub

    Private Sub LabelG7_Click(sender As Object, e As EventArgs) Handles LabelG7.Click
        _clsGameController.CellClickedEvent(7, 7)
    End Sub

    Private Sub LabelG7_Paint(sender As Object, e As PaintEventArgs) Handles LabelG7.Paint
        _clsGameController.PaintCellEvent(7, 7, e)
    End Sub

    Private Sub LabelH7_Click(sender As Object, e As EventArgs) Handles LabelH7.Click
        _clsGameController.CellClickedEvent(8, 7)
    End Sub

    Private Sub LabelH7_Paint(sender As Object, e As PaintEventArgs) Handles LabelH7.Paint
        _clsGameController.PaintCellEvent(8, 7, e)
    End Sub

    Private Sub LabelI7_Click(sender As Object, e As EventArgs) Handles LabelI7.Click
        _clsGameController.CellClickedEvent(9, 7)
    End Sub

    Private Sub LabelI7_Paint(sender As Object, e As PaintEventArgs) Handles LabelI7.Paint
        _clsGameController.PaintCellEvent(9, 7, e)
    End Sub

    Private Sub LabelA8_Click(sender As Object, e As EventArgs) Handles LabelA8.Click
        _clsGameController.CellClickedEvent(1, 8)
    End Sub

    Private Sub LabelA8_Paint(sender As Object, e As PaintEventArgs) Handles LabelA8.Paint
        _clsGameController.PaintCellEvent(1, 8, e)
    End Sub

    Private Sub LabelB8_Click(sender As Object, e As EventArgs) Handles LabelB8.Click
        _clsGameController.CellClickedEvent(2, 8)
    End Sub

    Private Sub LabelB8_Paint(sender As Object, e As PaintEventArgs) Handles LabelB8.Paint
        _clsGameController.PaintCellEvent(2, 8, e)
    End Sub

    Private Sub LabelC8_Click(sender As Object, e As EventArgs) Handles LabelC8.Click
        _clsGameController.CellClickedEvent(3, 8)
    End Sub

    Private Sub LabelC8_Paint(sender As Object, e As PaintEventArgs) Handles LabelC8.Paint
        _clsGameController.PaintCellEvent(3, 8, e)
    End Sub

    Private Sub LabelD8_Click(sender As Object, e As EventArgs) Handles LabelD8.Click
        _clsGameController.CellClickedEvent(4, 8)
    End Sub

    Private Sub LabelD8_Paint(sender As Object, e As PaintEventArgs) Handles LabelD8.Paint
        _clsGameController.PaintCellEvent(4, 8, e)
    End Sub

    Private Sub LabelE8_Click(sender As Object, e As EventArgs) Handles LabelE8.Click
        _clsGameController.CellClickedEvent(5, 8)
    End Sub

    Private Sub LabelE8_Paint(sender As Object, e As PaintEventArgs) Handles LabelE8.Paint
        _clsGameController.PaintCellEvent(5, 8, e)
    End Sub

    Private Sub LabelF8_Click(sender As Object, e As EventArgs) Handles LabelF8.Click
        _clsGameController.CellClickedEvent(6, 8)
    End Sub

    Private Sub LabelF8_Paint(sender As Object, e As PaintEventArgs) Handles LabelF8.Paint
        _clsGameController.PaintCellEvent(6, 8, e)
    End Sub

    Private Sub LabelG8_Click(sender As Object, e As EventArgs) Handles LabelG8.Click
        _clsGameController.CellClickedEvent(7, 8)
    End Sub

    Private Sub LabelG8_Paint(sender As Object, e As PaintEventArgs) Handles LabelG8.Paint
        _clsGameController.PaintCellEvent(7, 8, e)
    End Sub

    Private Sub LabelH8_Click(sender As Object, e As EventArgs) Handles LabelH8.Click
        _clsGameController.CellClickedEvent(8, 8)
    End Sub

    Private Sub LabelH8_Paint(sender As Object, e As PaintEventArgs) Handles LabelH8.Paint
        _clsGameController.PaintCellEvent(8, 8, e)
    End Sub

    Private Sub LabelI8_Click(sender As Object, e As EventArgs) Handles LabelI8.Click
        _clsGameController.CellClickedEvent(9, 8)
    End Sub

    Private Sub LabelI8_Paint(sender As Object, e As PaintEventArgs) Handles LabelI8.Paint
        _clsGameController.PaintCellEvent(9, 8, e)
    End Sub

    Private Sub LabelA9_Click(sender As Object, e As EventArgs) Handles LabelA9.Click
        _clsGameController.CellClickedEvent(1, 9)
    End Sub

    Private Sub LabelA9_Paint(sender As Object, e As PaintEventArgs) Handles LabelA9.Paint
        _clsGameController.PaintCellEvent(1, 9, e)
    End Sub

    Private Sub LabelB9_Click(sender As Object, e As EventArgs) Handles LabelB9.Click
        _clsGameController.CellClickedEvent(2, 9)
    End Sub

    Private Sub LabelB9_Paint(sender As Object, e As PaintEventArgs) Handles LabelB9.Paint
        _clsGameController.PaintCellEvent(2, 9, e)
    End Sub

    Private Sub LabelC9_Click(sender As Object, e As EventArgs) Handles LabelC9.Click
        _clsGameController.CellClickedEvent(3, 9)
    End Sub

    Private Sub LabelC9_Paint(sender As Object, e As PaintEventArgs) Handles LabelC9.Paint
        _clsGameController.PaintCellEvent(3, 9, e)
    End Sub

    Private Sub LabelD9_Click(sender As Object, e As EventArgs) Handles LabelD9.Click
        _clsGameController.CellClickedEvent(4, 9)
    End Sub

    Private Sub LabelD9_Paint(sender As Object, e As PaintEventArgs) Handles LabelD9.Paint
        _clsGameController.PaintCellEvent(4, 9, e)
    End Sub

    Private Sub LabelE9_Click(sender As Object, e As EventArgs) Handles LabelE9.Click
        _clsGameController.CellClickedEvent(5, 9)
    End Sub

    Private Sub LabelE9_Paint(sender As Object, e As PaintEventArgs) Handles LabelE9.Paint
        _clsGameController.PaintCellEvent(5, 9, e)
    End Sub

    Private Sub LabelF9_Click(sender As Object, e As EventArgs) Handles LabelF9.Click
        _clsGameController.CellClickedEvent(6, 9)
    End Sub

    Private Sub LabelF9_Paint(sender As Object, e As PaintEventArgs) Handles LabelF9.Paint
        _clsGameController.PaintCellEvent(6, 9, e)
    End Sub

    Private Sub LabelG9_Click(sender As Object, e As EventArgs) Handles LabelG9.Click
        _clsGameController.CellClickedEvent(7, 9)
    End Sub

    Private Sub LabelG9_Paint(sender As Object, e As PaintEventArgs) Handles LabelG9.Paint
        _clsGameController.PaintCellEvent(7, 9, e)
    End Sub

    Private Sub LabelH9_Click(sender As Object, e As EventArgs) Handles LabelH9.Click
        _clsGameController.CellClickedEvent(8, 9)
    End Sub

    Private Sub LabelH9_Paint(sender As Object, e As PaintEventArgs) Handles LabelH9.Paint
        _clsGameController.PaintCellEvent(8, 9, e)
    End Sub

    Private Sub LabelI9_Click(sender As Object, e As EventArgs) Handles LabelI9.Click
        _clsGameController.CellClickedEvent(9, 9)
    End Sub

    Private Sub LabelI9_Paint(sender As Object, e As PaintEventArgs) Handles LabelI9.Paint
        _clsGameController.PaintCellEvent(9, 9, e)
    End Sub

#End Region

#Region " Event Handlers: Game Controller "

    Private Sub _GameController_GameControllerEvent(sender As Object, e As GameControllerEventArgs) Handles _clsGameController.GameControllerEvent
        Select Case e.GameControllerEventType
            Case GameControllerEventType.GameTimer
                SetStatus = e.GameElapsedTime

            Case GameControllerEventType.GameCount
                SetGameCount(e.Level, e.Value)

            Case Else
                ' Invalid Game Controller Event Type

        End Select

    End Sub

#End Region

#End Region

#Region " Public Methods "

    Friend Sub DisplayGameCompletedForm(sElapsedTime As String)
        Dim fComplete As New frmComplete
        With fComplete
            .ElapsedTime = sElapsedTime                     ' Display elapsed time on form
            .TopMost = True                                 ' Set TopMost property to True
            .ShowDialog()                                   ' Show the form 
            .Dispose()                                      ' Dispose of form when user closes it
        End With
    End Sub

    Friend Function ShowNumberPanel(iCol As Int32, iRow As Int32) As Int32
        If Common.IsValidIndex(iCol, iRow) Then                                     ' Are indices valid?
            Dim X As Int32                                                          ' Yes, continue processing
            Dim Y As Int32
            With _uLabels(iCol, iRow)                                               ' Compute (X,Y) coordinates to move form to
                X = Me.Location.X + (.Parent.Parent.Location.X) + (.Width * iCol)
                Y = Me.Location.Y + (.Parent.Parent.Location.Y) + (.Height * iRow)
            End With
            Dim fInput As New frmInput
            With fInput
                .StartPosition = FormStartPosition.Manual                           ' Make sure start position is set to manual
                .Location = New Point(X, Y)                                         ' Move form
                .TopMost = True                                                     ' Set TopMost property to true
                .ShowDialog()                                                       ' Show form
                Dim iResult As Int32 = .ButtonSelected                              ' Save button selected
                .Dispose()                                                          ' Dispose of form
                Return iResult                                                      ' Return selected number
            End With
        End If
        Return -1                                                                   ' Invalid indices, return -1
    End Function

    Friend Sub EnableGameButtons(bFlag As Boolean, bShowPanel As Boolean)
        ' Enable or disable all the game related buttons
        ' Also, show or hide the panel that blocks the game
        ckbEnterNotes.Enabled = bFlag
        ckbShowAllNotes.Enabled = bFlag
        ckbShowSolution.Enabled = bFlag
        btnReset.Enabled = bFlag
        btnHint.Enabled = bFlag
        btnClear.Enabled = bFlag
        btnPrint.Enabled = bFlag
        pnHide.Visible = bShowPanel
    End Sub

    Friend Sub ShowHelp()
        Dim fHelp As New frmHelp
        With fHelp
            .TopMost = True
            .ShowDialog()
            .Dispose()
        End With
    End Sub

    Friend Sub InvalidateLabel(uIndex As CellIndex)
        If uIndex IsNot Nothing Then                ' Valid index object?
            With uIndex
                InvalidateLabel(.Col, .Row)         ' Yes, invalidate the label
            End With
        End If
    End Sub

    Friend Sub InvalidateLabel(iCol As Int32, iRow As Int32)
        If Common.IsValidIndex(iCol, iRow) Then     ' Are indices valid?
            _uLabels(iCol, iRow).Invalidate()       ' Yes, invalidate the label
        End If
    End Sub

    Friend Sub ClearAllCells()
        For Each Item As Label In _uLabels          ' Loop through the label array
            If Item IsNot Nothing Then              ' If there's a label pointer
                Item.Text = ""                      ' Then set the text value
            End If
        Next
    End Sub

    Friend Sub InvalidateAllLabels()
        For Each Item As Label In _uLabels          ' Loop through the label array
            If Item IsNot Nothing Then              ' If there's a label pointer
                Item.Invalidate()                   ' Invalidate the label to force a repaint
            End If
        Next
    End Sub

    Friend Sub ClearCell(iCol As Int32, iRow As Int32, e As PaintEventArgs)
        SetLabel(iCol, iRow, Color.Blue, "")                    ' Clear the label
        If Common.IsValidIndex(iCol, iRow) Then                 ' Are indices valid?
            e.Graphics.Clear(_uLabels(iCol, iRow).BackColor)    ' Yes, clear the label of any graphics using the label's backcolor
        End If
    End Sub

    Friend Sub SetLabel(iCol As Int32, iRow As Int32, uColor As Color, sText As String)
        If Common.IsValidIndex(iCol, iRow) Then     ' Are indices valid?
            With _uLabels(iCol, iRow)               ' Yes, continue processing
                .ForeColor = uColor                 ' Set the forecolor
                .Text = sText                       ' Display the text
            End With
        End If
    End Sub

    Friend Sub DrawNotes(iCol As Int32, iRow As Int32, e As PaintEventArgs, bNotes() As Boolean)
        If Common.IsValidIndex(iCol, iRow) Then                             ' Are indices valid?
            Dim drawFont As New Font("Arial", 8)                            ' Yes, create a font element
            For I As Int32 = 1 To 3                                         ' Loop through each number
                For J As Int32 = 1 To 3
                    Dim noteIndex As Int32 = J + ((I - 1) * 3)              ' Compute the index into the notes array (0 to 8)
                    If bNotes(noteIndex) Then                               ' Is the note field turned on?
                        With _uLabels(iCol, iRow)                           ' Yes, compute the location on the label to print the number
                            Dim X As Int32 = CInt((.Width / 3) * (J - 1))   ' Divide width by 3 and multiply by index
                            Dim Y As Int32 = CInt((.Height / 3) * (I - 1))  ' Divide height by 3 and multiply by index
                            e.Graphics.DrawString(noteIndex.ToString, drawFont, Brushes.Black, X, Y)
                        End With
                    End If
                Next
            Next
            drawFont.Dispose()                                              ' Get rid of the font object
        End If
    End Sub

    Friend Sub DrawBorder(iCol As Int32, iRow As Int32, e As PaintEventArgs, bHighlight As Boolean)
        If Common.IsValidIndex(iCol, iRow) Then                                 ' Are indices valid?
            With _uLabels(iCol, iRow)                                           ' Yes, continue processing
                Dim highlightPen As Pen
                If bHighlight Then                                              ' Highlight cell?
                    highlightPen = New Pen(Brushes.CadetBlue, 4)                ' Yes, use this color
                Else
                    highlightPen = New Pen(.BackColor, 4)                       ' No, use the background color
                End If
                e.Graphics.DrawRectangle(highlightPen, 0, 0, .Width, .Height)   ' Draw a rectangle around the label
                highlightPen.Dispose()                                          ' Dispose the pen
            End With
        End If
    End Sub

#End Region

#Region " Private Methods "

#Region " Private Methods: Form Init Routines "

    Private Sub InitForm()
        InitComboBox()
        InitLabelArray()
        btnPrint.Visible = False    ' Hide the print button until we code the print routines
        btnStart.Enabled = False    ' Disable Start Button when the application first loads
    End Sub

    Private Sub InitLabelArray()
        InitLabelArray(1, 1, LabelA1)
        InitLabelArray(1, 2, LabelA2)
        InitLabelArray(1, 3, LabelA3)
        InitLabelArray(1, 4, LabelA4)
        InitLabelArray(1, 5, LabelA5)
        InitLabelArray(1, 6, LabelA6)
        InitLabelArray(1, 7, LabelA7)
        InitLabelArray(1, 8, LabelA8)
        InitLabelArray(1, 9, LabelA9)

        InitLabelArray(2, 1, LabelB1)
        InitLabelArray(2, 2, LabelB2)
        InitLabelArray(2, 3, LabelB3)
        InitLabelArray(2, 4, LabelB4)
        InitLabelArray(2, 5, LabelB5)
        InitLabelArray(2, 6, LabelB6)
        InitLabelArray(2, 7, LabelB7)
        InitLabelArray(2, 8, LabelB8)
        InitLabelArray(2, 9, LabelB9)

        InitLabelArray(3, 1, LabelC1)
        InitLabelArray(3, 2, LabelC2)
        InitLabelArray(3, 3, LabelC3)
        InitLabelArray(3, 4, LabelC4)
        InitLabelArray(3, 5, LabelC5)
        InitLabelArray(3, 6, LabelC6)
        InitLabelArray(3, 7, LabelC7)
        InitLabelArray(3, 8, LabelC8)
        InitLabelArray(3, 9, LabelC9)

        InitLabelArray(4, 1, LabelD1)
        InitLabelArray(4, 2, LabelD2)
        InitLabelArray(4, 3, LabelD3)
        InitLabelArray(4, 4, LabelD4)
        InitLabelArray(4, 5, LabelD5)
        InitLabelArray(4, 6, LabelD6)
        InitLabelArray(4, 7, LabelD7)
        InitLabelArray(4, 8, LabelD8)
        InitLabelArray(4, 9, LabelD9)

        InitLabelArray(5, 1, LabelE1)
        InitLabelArray(5, 2, LabelE2)
        InitLabelArray(5, 3, LabelE3)
        InitLabelArray(5, 4, LabelE4)
        InitLabelArray(5, 5, LabelE5)
        InitLabelArray(5, 6, LabelE6)
        InitLabelArray(5, 7, LabelE7)
        InitLabelArray(5, 8, LabelE8)
        InitLabelArray(5, 9, LabelE9)

        InitLabelArray(6, 1, LabelF1)
        InitLabelArray(6, 2, LabelF2)
        InitLabelArray(6, 3, LabelF3)
        InitLabelArray(6, 4, LabelF4)
        InitLabelArray(6, 5, LabelF5)
        InitLabelArray(6, 6, LabelF6)
        InitLabelArray(6, 7, LabelF7)
        InitLabelArray(6, 8, LabelF8)
        InitLabelArray(6, 9, LabelF9)

        InitLabelArray(7, 1, LabelG1)
        InitLabelArray(7, 2, LabelG2)
        InitLabelArray(7, 3, LabelG3)
        InitLabelArray(7, 4, LabelG4)
        InitLabelArray(7, 5, LabelG5)
        InitLabelArray(7, 6, LabelG6)
        InitLabelArray(7, 7, LabelG7)
        InitLabelArray(7, 8, LabelG8)
        InitLabelArray(7, 9, LabelG9)

        InitLabelArray(8, 1, LabelH1)
        InitLabelArray(8, 2, LabelH2)
        InitLabelArray(8, 3, LabelH3)
        InitLabelArray(8, 4, LabelH4)
        InitLabelArray(8, 5, LabelH5)
        InitLabelArray(8, 6, LabelH6)
        InitLabelArray(8, 7, LabelH7)
        InitLabelArray(8, 8, LabelH8)
        InitLabelArray(8, 9, LabelH9)

        InitLabelArray(9, 1, LabelI1)
        InitLabelArray(9, 2, LabelI2)
        InitLabelArray(9, 3, LabelI3)
        InitLabelArray(9, 4, LabelI4)
        InitLabelArray(9, 5, LabelI5)
        InitLabelArray(9, 6, LabelI6)
        InitLabelArray(9, 7, LabelI7)
        InitLabelArray(9, 8, LabelI8)
        InitLabelArray(9, 9, LabelI9)
    End Sub

    Friend Sub InitLabelArray(iCol As Int32, iRow As Int32, lLabel As Label)
        If Common.IsValidIndex(iCol, iRow) Then                     ' If the iCol and iRow values are good
            _uLabels(iCol, iRow) = lLabel                           ' Save the pointer to the corresponding label
        End If
    End Sub


    Private Sub InitComboBox()
        With cbDifficultyLevel
            ' These should match the PuzzelLevelEnum, except for the first value (Invalid)
            .Items.Add("Very Easy")
            .Items.Add("Easy")
            .Items.Add("Medium")
            .Items.Add("Hard")
            .Items.Add("Expert")
            .SelectedIndex = 0         ' Show first item as the default selected one.  Otherwise the dropdown will show blank.
        End With
    End Sub

#End Region

#Region " Private Methods: Form Action Routines "

    Private Sub ClearForm()
        _clsGameController.ClearForm()                      ' Clear the form
        GameLevelCount(GameLevelEnum.VeryEasy) = 0          ' Set all the game counts to zero
        GameLevelCount(GameLevelEnum.Easy) = 0
        GameLevelCount(GameLevelEnum.Medium) = 0
        GameLevelCount(GameLevelEnum.Hard) = 0
        GameLevelCount(GameLevelEnum.Expert) = 0
    End Sub

    Private Sub SetStatusText(sMsg As String)
        If lblStatus.InvokeRequired Then                                    ' Does the Status Label require an Invoke?
            Dim callback As New SetStatusCallback(AddressOf SetStatusText)  ' Yes, then use the Callback routine
            Me.Invoke(callback, New Object() {sMsg})                        ' Invoke the callback routine
        Else
            lblStatus.Text = sMsg                                           ' No, then just set the text.
        End If
    End Sub

    Private Sub SetStatusBarText(sMsg As String)
        If ssStatus.InvokeRequired Then                                             ' Does the Status Bar require an Invoke?
            Dim callback As New SetStatusBarCallback(AddressOf SetStatusBarText)    ' Yes, then use the callback routine
            Me.Invoke(callback, New Object() {sMsg})                                ' Invoke the callback routine
        Else
            tssLabel.Text = sMsg                                                    ' No, then just set the text.
        End If
    End Sub

    Private Sub SetGameCount(eLevel As GameLevelEnum, value As Int32)
        Select Case eLevel
            Case GameLevelEnum.VeryEasy
                SetGameCount(tssVeryEasyCount, value)

            Case GameLevelEnum.Easy
                SetGameCount(tssEasyCount, value)

            Case GameLevelEnum.Medium
                SetGameCount(tssMediumCount, value)

            Case GameLevelEnum.Hard
                SetGameCount(tssHardCount, value)

            Case GameLevelEnum.Expert
                SetGameCount(tssExpertCount, value)

        End Select
    End Sub

    Private Sub SetGameCount(tssLabel As ToolStripLabel, value As Int32)
        If ssStatus.InvokeRequired Then                                             ' Does the Status Bar require an Invoke?
            Dim callback As New SetGameCountCallback(AddressOf SetGameCount)        ' Yes, then use the callback routine
            Me.Invoke(callback, New Object() {tssLabel, value})                     ' Invoke the callback routine
        Else
            tssLabel.Text = value.ToString                                          ' No, then just set the text.
        End If
    End Sub

    Private Sub StartGameController()
        _clsGameController = New GameController(Me)                         ' Load the Controller and pass it the main View
    End Sub

#End Region

#End Region

End Class
