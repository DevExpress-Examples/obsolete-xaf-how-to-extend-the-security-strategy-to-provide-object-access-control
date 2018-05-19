Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace Solution41.Module
	<DefaultClassOptions> _
	Public Class Client
		Inherits Person
		Public Sub New(ByVal s As Session)
			MyBase.New(s)
		End Sub
	End Class

	<DefaultClassOptions> _
	Public Class SaleTask
		Inherits Task
		Public Sub New(ByVal s As Session)
			MyBase.New(s)
		End Sub
	End Class
End Namespace
