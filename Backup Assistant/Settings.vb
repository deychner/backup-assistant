Imports System.Configuration

Namespace My
    
    'This class allows you to handle specific events on the settings class:
    ' The SettingChanging event is raised before a setting's value is changed.
    ' The PropertyChanged event is raised after a setting's value is changed.
    ' The SettingsLoaded event is raised after the setting values are loaded.
    ' The SettingsSaving event is raised before the setting values are saved.
    Partial Friend NotInheritable Class MySettings

        Friend Sub Handle_SettingsLoaded(sender As Object, e As SettingsLoadedEventArgs) Handles Me.SettingsLoaded
            Try
                ' Attempt to load previous version
                If Me.UpgradeRequired Then
                    Me.Upgrade()
                    Me.UpgradeRequired = False
                    Me.Save()
                End If
            Catch ex As ConfigurationException
                ' The configuration file could not be parsed.
            End Try
        End Sub

    End Class
End Namespace
