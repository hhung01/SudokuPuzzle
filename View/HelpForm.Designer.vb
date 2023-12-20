<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmHelp
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtHelp = New System.Windows.Forms.TextBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'txtHelp
        '
        Me.txtHelp.Cursor = System.Windows.Forms.Cursors.Default
        Me.txtHelp.Font = New System.Drawing.Font("Verdana", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtHelp.Location = New System.Drawing.Point(15, 15)
        Me.txtHelp.Multiline = True
        Me.txtHelp.Name = "txtHelp"
        Me.txtHelp.ReadOnly = True
        Me.txtHelp.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtHelp.Size = New System.Drawing.Size(449, 254)
        Me.txtHelp.TabIndex = 0
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(387, 275)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(77, 30)
        Me.btnOK.TabIndex = 1
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(12, 275)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(369, 32)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "This software is copyrighted and released under the GNU General Public License v3" & _
    ".  Copyright (c) 2014 Han Hung."
        '
        'frmHelp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(476, 312)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtHelp)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmHelp"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Sudoku Help"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtHelp As System.Windows.Forms.TextBox
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
End Class
