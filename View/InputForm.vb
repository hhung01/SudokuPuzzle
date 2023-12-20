'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Friend Class frmInput

#Region " Variables "

    Private _iResult As Int32 = -1

#End Region

#Region " Property "

    Friend ReadOnly Property ButtonSelected As Int32
        Get
            Return _iResult
        End Get
    End Property

#End Region

#Region " Event Handlers "

    Private Sub btn1_Click(sender As Object, e As EventArgs) Handles btn1.Click
        ProcessButton(1)
    End Sub

    Private Sub btn2_Click(sender As Object, e As EventArgs) Handles btn2.Click
        ProcessButton(2)
    End Sub

    Private Sub btn3_Click(sender As Object, e As EventArgs) Handles btn3.Click
        ProcessButton(3)
    End Sub

    Private Sub btn4_Click(sender As Object, e As EventArgs) Handles btn4.Click
        ProcessButton(4)
    End Sub

    Private Sub btn5_Click(sender As Object, e As EventArgs) Handles btn5.Click
        ProcessButton(5)
    End Sub

    Private Sub btn6_Click(sender As Object, e As EventArgs) Handles btn6.Click
        ProcessButton(6)
    End Sub

    Private Sub btn7_Click(sender As Object, e As EventArgs) Handles btn7.Click
        ProcessButton(7)
    End Sub

    Private Sub btn8_Click(sender As Object, e As EventArgs) Handles btn8.Click
        ProcessButton(8)
    End Sub

    Private Sub btn9_Click(sender As Object, e As EventArgs) Handles btn9.Click
        ProcessButton(9)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Hide()
    End Sub

#End Region

#Region " Private Methods "

    Private Sub ProcessButton(value As Int32)
        _iResult = value
        Me.Hide()
    End Sub

#End Region

End Class