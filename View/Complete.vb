'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Friend Class frmComplete

    ' TODO: Add some kind of fireworks display in the background.

#Region " Properties "

    Friend Property ElapsedTime As String

#End Region

#Region " Event Handlers "

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub

    Private Sub frmComplete_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblTime.Text = "Your time is: " & ElapsedTime
    End Sub

#End Region

End Class
