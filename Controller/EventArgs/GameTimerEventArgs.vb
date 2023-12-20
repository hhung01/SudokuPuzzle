'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Namespace Controller

    Friend Class GameTimerEventArgs
        Inherits EventArgs

#Region " Variables "

        Private _sElapsedTime As String

#End Region

#Region " Public Properties "

        Friend ReadOnly Property ElapsedTime As String
            Get
                Return _sElapsedTime
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New(sElapsedTime As String)
            _sElapsedTime = sElapsedTime
        End Sub

#End Region

    End Class

End Namespace