Imports Microsoft.VisualBasic
Imports System

Imports DevExpress.ExpressApp.Updating
Imports DevExpress.Xpo
Imports DevExpress.Data.Filtering
Imports DevExpress.Persistent.BaseImpl

Namespace Solution41.Module
	Public Class Updater
		Inherits ModuleUpdater
		Public Sub New(ByVal session As Session, ByVal currentDBVersion As Version)
			MyBase.New(session, currentDBVersion)
		End Sub
		Public Overrides Sub UpdateDatabaseAfterUpdateSchema()
			MyBase.UpdateDatabaseAfterUpdateSchema()

			Dim admin As UserWithLicense = Session.FindObject(Of UserWithLicense)(CriteriaOperator.Parse("IsAdministrator==true && IsActive==true"))
			If admin Is Nothing Then
				admin = New UserWithLicense(Session)
				admin.UserName = "Admin"
				admin.IsActive = True
				admin.IsAdministrator = True
				admin.Save()
			End If
			Dim guest As UserWithLicense = Session.FindObject(Of UserWithLicense)(CriteriaOperator.Parse("UserName=='Guest'"))
			If guest Is Nothing Then
				guest = New UserWithLicense(Session)
				guest.UserName = "Guest"
				guest.IsActive = True
				guest.Save()
			End If
			Dim user As UserWithLicense = Session.FindObject(Of UserWithLicense)(CriteriaOperator.Parse("UserName=='User'"))
			If user Is Nothing Then
				user = New UserWithLicense(Session)
				user.UserName = "User"
				user.IsActive = True
				user.License = "Solution41.Module.Client;Solution41.Module.SaleTask;"
				user.Save()
			End If
		End Sub
	End Class
End Namespace
