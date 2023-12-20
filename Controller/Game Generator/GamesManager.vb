'
' Copyright (c) 2014 Han Hung
' 
' This program is free software; it is distributed under the terms
' of the GNU General Public License v3 as published by the Free
' Software Foundation.
'
' http://www.gnu.org/licenses/gpl-3.0.html
' 

Namespace Controller.GameGenerator

    Friend Class GamesManager
        ' This class manages all the different games that are generated
        ' When the UI requests a game for a particular level, the request
        ' comes here.

#Region " Variables, Constants, And other Declarations "

#Region " Constants "

        ' This should match GameLevelEnum
        Private _cMaxLevels As Int32 = 4

#End Region

#Region " Variables "

        Private _Games(_cMaxLevels) As GameCollection

        Private WithEvents _GamesVeryEasy As GameCollection
        Private WithEvents _GamesEasy As GameCollection
        Private WithEvents _GamesMedium As GameCollection
        Private WithEvents _GamesHard As GameCollection
        Private WithEvents _GamesExpert As GameCollection

#End Region

#Region " Other Declarations "

        Friend Delegate Sub GamesManagerEventHandler(sender As Object, e As GameManagerEventArgs)
        Friend Event GamesManagerEvent As GamesManagerEventHandler

#End Region

#End Region

#Region " Public Properties "

        Friend ReadOnly Property GameCount(eLevel As GameLevelEnum) As Int32
            Get
                Return _Games(eLevel).GameCount
            End Get
        End Property

#End Region

#Region " Constructors "

        Friend Sub New()
            InitializeClass()                               ' Init any class level stuff
        End Sub

#End Region

#Region " Event Handlers "

        Private Sub _GamesEasy_GameManagerEvent(sender As Object, e As GameManagerEventArgs) Handles _GamesEasy.GameManagerEvent
            RaiseEvent GamesManagerEvent(Me, e)
        End Sub

        Private Sub _GamesExpert_GameManagerEvent(sender As Object, e As GameManagerEventArgs) Handles _GamesExpert.GameManagerEvent
            RaiseEvent GamesManagerEvent(Me, e)
        End Sub

        Private Sub _GamesHard_GameManagerEvent(sender As Object, e As GameManagerEventArgs) Handles _GamesHard.GameManagerEvent
            RaiseEvent GamesManagerEvent(Me, e)
        End Sub

        Private Sub _GamesMedium_GameManagerEvent(sender As Object, e As GameManagerEventArgs) Handles _GamesMedium.GameManagerEvent
            RaiseEvent GamesManagerEvent(Me, e)
        End Sub

        Private Sub _GamesVeryEasy_GameManagerEvent(sender As Object, e As GameManagerEventArgs) Handles _GamesVeryEasy.GameManagerEvent
            RaiseEvent GamesManagerEvent(Me, e)
        End Sub

#End Region

#Region " Methods "

#Region " Public Methods "

        Friend Sub StopGamesManager()
            StopBackgroundTasks()                                           ' Stop all background tasks
            SaveGames()                                                     ' Save all the games
        End Sub

        Friend Function GetGame(eLevel As GameLevelEnum) As Model.CellStateClass(,)
            Return _Games(eLevel).GetGame
        End Function

#End Region

#Region " Private Methods "

        Private Sub InitializeClass()
            _GamesVeryEasy = New GameCollection(GameLevelEnum.VeryEasy)
            _GamesEasy = New GameCollection(GameLevelEnum.Easy)
            _GamesMedium = New GameCollection(GameLevelEnum.Medium)
            _GamesHard = New GameCollection(GameLevelEnum.Hard)
            _GamesExpert = New GameCollection(GameLevelEnum.Expert)
            _Games(0) = _GamesVeryEasy
            _Games(1) = _GamesEasy
            _Games(2) = _GamesMedium
            _Games(3) = _GamesHard
            _Games(4) = _GamesExpert
            LoadGames()                                                     ' Load games.
            StartBackgroundTasks()                                          ' Start the background tasks.
        End Sub

        Private Sub StopBackgroundTasks()
            For Each Item As GameCollection In _Games                       ' Loop through the array of Games
                Item.StopThread()                                           ' Stop each background thread
            Next
        End Sub

        Private Sub StartBackgroundTasks()
            For Each Item As GameCollection In _Games                       ' Loop through each array of games
                Item.StartThread()                                          ' Start the background thread
            Next
        End Sub

        Private Sub SaveGames()
            My.Settings.GamesLevel0 = _Games(0).SaveGames()                 ' For each level, save all the games created
            My.Settings.GamesLevel1 = _Games(1).SaveGames()
            My.Settings.GamesLevel2 = _Games(2).SaveGames()
            My.Settings.GamesLevel3 = _Games(3).SaveGames()
            My.Settings.GamesLevel4 = _Games(4).SaveGames()
        End Sub

        Private Sub LoadGames()
            _Games(0).LoadGames(My.Settings.GamesLevel0)                    ' For each level, load any pre-created games
            _Games(1).LoadGames(My.Settings.GamesLevel1)
            _Games(2).LoadGames(My.Settings.GamesLevel2)
            _Games(3).LoadGames(My.Settings.GamesLevel3)
            _Games(4).LoadGames(My.Settings.GamesLevel4)
        End Sub

#End Region

#End Region

    End Class

End Namespace