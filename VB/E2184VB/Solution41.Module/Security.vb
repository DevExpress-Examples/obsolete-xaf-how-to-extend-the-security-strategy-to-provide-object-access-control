Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.Xpo
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Persistent.Base.Security
Imports System.Security
Imports DevExpress.ExpressApp.Security
Imports DevExpress.Persistent.Base.General
Imports System.ComponentModel

Namespace Solution41.Module

	Public Interface ILicensedUser
	Inherits ISimpleUser
		ReadOnly Property License() As String
	End Interface

	<DefaultClassOptions> _
	Public Class UserWithLicense
		Inherits SimpleUser
		Implements ILicensedUser

		Private license_Renamed As String

		Public Sub New(ByVal s As Session)
			MyBase.New(s)
		End Sub

		#Region "ILicensedUser Members"

        <Browsable(False)> _
        Public Property License() As String
            Get
                Return license_Renamed
            End Get
            Set(ByVal value As String)
                SetPropertyValue("License", license_Renamed, value)
            End Set
        End Property

        <Browsable(False)> _
        Public ReadOnly Property License_() As String Implements ILicensedUser.License
            Get
                Return license_Renamed
            End Get
        End Property
#End Region
	End Class

	Public Class SecuritySimpleEx
		Inherits SecuritySimple
		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal userType As Type, ByVal authentication As AuthenticationBase)
			MyBase.New(userType, authentication)
		End Sub
		Public Overrides Overloads Function IsSecurityMember(ByVal type As Type, ByVal memberName As String) As Boolean
			Return ((GetType(ILicensedUser).IsAssignableFrom(type) AndAlso (GetType(ILicensedUser).GetMember(memberName).Length > 0)) OrElse MyBase.IsSecurityMember(type, memberName))
		End Function
		Protected Overrides Overloads Function ReloadPermissions() As PermissionSet
			Dim [set] As PermissionSet = MyBase.ReloadPermissions()
			If Me.User.IsActive Then
				MyBase.IsGrantedForNonExistentPermission = True
				[set].AddPermission(New ObjectAccessPermission(GetType(Object), ObjectAccess.AllAccess))

				[set].AddPermission(New ObjectAccessPermission(GetType(IPropertyBag), ObjectAccess.AllAccess))

				[set].AddPermission(New ObjectAccessPermission(GetType(IXPSimpleObject), ObjectAccess.NoAccess))

				[set].AddPermission(New ObjectAccessPermission(Me.UserType, ObjectAccess.AllAccess))

                [set].AddPermission(New EditModelPermission(CType(IIf(Me.User.IsAdministrator, ModelAccessModifier.Allow, ModelAccessModifier.Deny), ModelAccessModifier)))

				If (Not Me.User.IsAdministrator) Then
					[set].AddPermission(New ObjectAccessPermission(Me.UserType, ObjectAccess.ChangeAccess, ObjectAccessModifier.Deny))

					If (Not Me.AllowNonAdministratorNavigateToUsers) Then
						[set].AddPermission(New ObjectAccessPermission(Me.UserType, ObjectAccess.Navigate, ObjectAccessModifier.Deny))

					End If
				End If
				Dim luser As ILicensedUser = TryCast(Me.User, ILicensedUser)
				If luser IsNot Nothing AndAlso (Not String.IsNullOrEmpty(luser.License)) Then
					AddLicensedPermissions([set], DecryptLicense(luser.License))
				End If
				Return [set]
			End If
			MyBase.IsGrantedForNonExistentPermission = False
			Return [set]
		End Function

		Private Function DecryptLicense(ByVal license As String) As String
			Return license
		End Function

		Private Sub AddLicensedPermissions(ByVal [set] As PermissionSet, ByVal license As String)
			If license Is Nothing Then
				Return
			End If
			Dim items() As String = license.Split(";"c)
			For Each item As String In items
				Dim type As Type = Type.GetType(item)
				If type IsNot Nothing Then
					Dim p As New ObjectAccessPermission(type, ObjectAccess.AllAccess)
					[set].AddPermission(p)

				End If
			Next item
		End Sub
	End Class
End Namespace
