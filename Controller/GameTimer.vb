'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 
Imports System.Timers

Namespace Controller

    Friend Class GameTimer
        ' All this class does is provide a timer for the game.  When the user
        ' starts a game, it will start counting how long the user takes to
        ' solve the puzzle.  It will update the game time every second.
        ' When the game is over, it will maintain the elapsed time so that
        ' it can be accessed afterwards.

#Region " Variables, Constants, And other Declarations "

#Region " Constants "

        Private Const _cInitialTime As String = "00:00:00"  ' Initial time game started
        Private Const _cTimeFormat As String = "hh\:mm\:ss" ' Format string for the time
        Private Const _cInterval As Int32 = 1000            ' Update the time display every second or every 1,000 milliseconds

#End Region

#Region " Variables "

        Private _dtStartTime As DateTime                    ' Holds the date/time when the game started
        Private _sElapsedTime As String                     ' Holds the final elapsed time the user took to complete the puzzle

        Private WithEvents _uTimer As Timer                 ' Timer variable

#End Region

#Region " Other Declarations "

        Friend Delegate Sub GameTimerEventHandler(sender As Object, e As GameTimerEventArgs)
        Friend Event GameTimerEvent As GameTimerEventHandler

#End Region

#End Region

#Region " Public Properties "

        Friend ReadOnly Property ElapsedTime As String
            Get
                Return _sElapsedTime                        ' Readonly property that returns the elapsed time
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New()
            _sElapsedTime = _cInitialTime                   ' Set the initial elapsed time string to all zeroes
        End Sub

#End Region

#Region " Event Handlers "

        Private Sub _Timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _uTimer.Elapsed
            ComputeElapsedTime()                            ' Compute the elapsed time
            RaiseGameTimerEvent(_sElapsedTime)              ' Send an event to the Game Controller
        End Sub

#End Region

#Region " Public Methods "

        Friend Sub StartTimer()
            If _uTimer Is Nothing Then                      ' If Timer object is nothing
                _uTimer = New Timer(_cInterval)             ' Instantiate a new Timer object with a fixed interval
            End If
            _dtStartTime = DateTime.Now                     ' Save the current date/time
            _uTimer.AutoReset = True                        ' Set the AutoReset flag to true so that the Timer will keep ticking
            _uTimer.Enabled = True                          ' Start the timer
            RaiseGameTimerEvent(_cInitialTime)              ' Raise the initial event and set time to 00:00:00
        End Sub

        Friend Sub StopTimer()
            Try
                If (_uTimer IsNot Nothing) AndAlso (_uTimer.Enabled) Then   ' If there is a Timer object and it is running
                    _uTimer.Enabled = False                                 ' Stop the timer
                    ComputeElapsedTime()                                    ' Compute elapsed time
                    RaiseGameTimerEvent("")                                 ' Blank out the display
                End If
            Catch ex As Exception
                ' TODO: What to do here?
            Finally
                _uTimer = Nothing                                           ' Release the Timer object
            End Try
        End Sub

        Friend Sub PauseTimer()
            If (_uTimer IsNot Nothing) AndAlso (_uTimer.Enabled) Then   ' If there is a Timer object and it is running
                _uTimer.Enabled = False                                 ' Then stop the timer
            End If
        End Sub

        Friend Sub ResumeTimer()
            If (_uTimer IsNot Nothing) Then                             ' If there is a Timer object 
                LoadPreviousTime()                                      ' Load the previous time
                _uTimer.Enabled = True                                  ' Start the timer
            End If

        End Sub

        Friend Sub LoadPreviousTime()
            Dim diff As TimeSpan = My.Settings.ElapsedTime          ' Retrieve Elapsed Time from the saved settings
            _dtStartTime = DateTime.Now - diff                      ' Subtract it from the current time to get a new start time
        End Sub

        Friend Sub ResetTimer()
            If _uTimer IsNot Nothing Then                           ' If there is a Timer object
                _uTimer.Enabled = False                             ' Stop the timer
                _dtStartTime = DateTime.Now                         ' Save the current date/time
                _uTimer.Enabled = True                              ' Start the timer
                RaiseGameTimerEvent(_cInitialTime)                  ' Raise the initial event and set time to 00:00:00
            End If
        End Sub

#End Region

#Region " Private Methods "

        Private Sub RaiseGameTimerEvent(sMessage As String)
            Dim eTimerEventArgs As New GameTimerEventArgs(sMessage)     ' Get an event structure
            RaiseEvent GameTimerEvent(Me, eTimerEventArgs)              ' Raise an event to let the controller know
        End Sub

        Private Sub ComputeElapsedTime()
            Try
                Dim tDiff As TimeSpan = DateTime.Now - _dtStartTime     ' Calculate the time elapsed since we started
                My.Settings.ElapsedTime = tDiff                         ' Save the elapsed time to the settings
                _sElapsedTime = tDiff.ToString(_cTimeFormat)            ' Convert it to a string as save it
            Catch ex As Exception
                ' TODO: What to do here?
                _sElapsedTime = _cInitialTime                           ' Error computing, just default to 00:00:00
            End Try
        End Sub

#End Region

    End Class

End Namespace
