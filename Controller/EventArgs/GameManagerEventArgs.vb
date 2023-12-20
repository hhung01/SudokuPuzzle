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

    Friend Class GameManagerEventArgs
        Inherits EventArgs

#Region " Variables "

        Private _eLevel As GameLevelEnum
        Private _iValue As Int32

#End Region

#Region " Public Properties "

        Friend ReadOnly Property Level As GameLevelEnum
            Get
                Return _eLevel
            End Get
        End Property

        Friend ReadOnly Property Value As Int32
            Get
                Return _iValue
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New(eLevel As GameLevelEnum, value As Int32)
            _eLevel = eLevel
            _iValue = value
        End Sub

#End Region

    End Class

End Namespace
