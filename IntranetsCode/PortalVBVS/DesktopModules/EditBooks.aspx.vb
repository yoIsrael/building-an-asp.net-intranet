
Imports System
Namespace ASPNetPortal

   Public Class EditBooks
      Inherits System.Web.UI.Page
      Protected WithEvents TitleField As System.Web.UI.WebControls.TextBox
      Protected WithEvents Req1 As System.Web.UI.WebControls.RequiredFieldValidator
      Protected WithEvents Req2 As System.Web.UI.WebControls.RequiredFieldValidator
      Protected WithEvents updateButton As System.Web.UI.WebControls.LinkButton
      Protected WithEvents cancelButton As System.Web.UI.WebControls.LinkButton
      Protected WithEvents deleteButton As System.Web.UI.WebControls.LinkButton
      Protected WithEvents CreatedBy As System.Web.UI.WebControls.Label
      Protected WithEvents CreatedDate As System.Web.UI.WebControls.Label

      Private itemId As Integer = 0
      Protected WithEvents ImageUrlField As System.Web.UI.WebControls.TextBox
      Protected WithEvents AuthorsField As System.Web.UI.WebControls.TextBox
      Protected WithEvents ISBNField As System.Web.UI.WebControls.TextBox
      Protected WithEvents PriceField As System.Web.UI.WebControls.TextBox
      Protected WithEvents BuyLinkField As System.Web.UI.WebControls.TextBox
      Protected WithEvents Req3 As System.Web.UI.WebControls.RequiredFieldValidator
      Protected WithEvents Req4 As System.Web.UI.WebControls.RequiredFieldValidator
      Protected WithEvents Req5 As System.Web.UI.WebControls.RequiredFieldValidator
      Protected WithEvents ColumnsField As System.Web.UI.WebControls.TextBox
      Protected WithEvents ColumnsFieldValidator As System.Web.UI.WebControls.RangeValidator
      Private moduleId As Integer = 0
      Protected WithEvents updateGlobalButton As System.Web.UI.WebControls.LinkButton
      Private settings As Hashtable

#Region " Web Form Designer Generated Code "

      'This call is required by the Web Form Designer.
      <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

      End Sub

      Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
         'CODEGEN: This method call is required by the Web Form Designer
         'Do not modify it using the code editor.
         InitializeComponent()
      End Sub

#End Region

      '****************************************************************
      '
      ' The Page_Load event on this Page is used to obtain the ModuleId
      ' and ItemId of the announcement to edit.
      '
      ' It then uses the ASPNetPortal.BooksDB() data component
      ' to populate the page's edit controls with the annoucement details.
      '
      '****************************************************************

      Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

         ' Determine ModuleId of Books Portal Module
         moduleId = Int32.Parse(Request.Params("Mid"))

         ' Verify that the current user has access to edit this module
         If PortalSecurity.HasEditPermissions(moduleId) = False Then
            Response.Redirect("~/Admin/EditAccessDenied.aspx")
         End If

         ' Determine ItemId of Nook to Update
         If Not (Request.Params("ItemId") Is Nothing) Then
            itemId = Int32.Parse(Request.Params("ItemId"))
         End If

         ' If the page is being requested the first time, determine if an
         ' book itemId value is specified, and if so populate page
         ' contents with the book details
         If Page.IsPostBack = False Then

            If itemId <> 0 Then

               ' Obtain a single row of announcement information
               Dim booksDB As New ASPNetPortal.BooksDB()
               Dim dr As SqlDataReader = booksDB.GetSingleBook(itemId)

               ' Load first row into DataReader
               dr.Read()

               TitleField.Text = CType(dr("Title"), String)
               ImageUrlField.Text = CType(dr("ImageUrl"), String)
               AuthorsField.Text = CType(dr("Authors"), String)
               ISBNField.Text = CType(dr("ISBN"), String)
               PriceField.Text = CType(dr("Price"), String)
               BuyLinkField.Text = CType(dr("BuyLink"), String)
               CreatedBy.Text = CType(dr("CreatedByUser"), String)
               CreatedDate.Text = CType(dr("CreatedDate"), DateTime).ToShortDateString()

               ' Close the datareader
               dr.Close()

            End If

            ' Get global settings from the database
            settings = PortalSettings.GetModuleSettings(moduleId)

            Dim repeatColumns As String = CType(settings("columns"), String)
            If repeatColumns = Nothing Then
               ColumnsField.Text = "0"
            Else
               ColumnsField.Text = repeatColumns
            End If

            ' Store URL Referrer to return to portal
            ViewState("UrlReferrer") = Request.UrlReferrer.ToString()

         End If

      End Sub


      '****************************************************************
      '
      ' The UpdateBtn_Click event handler on this Page is used to either
      ' create or update an announcement.  It  uses the ASPNetPortal.AnnouncementsDB()
      ' data component to encapsulate all data functionality.
      '
      '****************************************************************

      Private Sub UpdateBtn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles updateButton.Click

         ' Only Update if the Entered Data is Valid
         If Page.IsValid = True Then

            ' Create an instance of the Books DB component
            Dim booksDB As New ASPNetPortal.BooksDB()

            If itemId = 0 Then

               ' Add the announcement within the Books table
               booksDB.AddBook(moduleId, itemId, Context.User.Identity.Name, TitleField.Text, ImageUrlField.Text, AuthorsField.Text, PriceField.Text, ISBNField.Text, BuyLinkField.Text)

            Else

               ' Update the announcement within the Books table
               booksDB.UpdateBook(moduleId, itemId, Context.User.Identity.Name, TitleField.Text, ImageUrlField.Text, AuthorsField.Text, PriceField.Text, ISBNField.Text, BuyLinkField.Text)

            End If

            ' Redirect back to the portal home page
            Response.Redirect(CType(ViewState("UrlReferrer"), String))

         End If

      End Sub


      '****************************************************************
      '
      ' The DeleteBtn_Click event handler on this Page is used to delete an
      ' an announcement.  It  uses the ASPNetPortal.AnnouncementsDB()
      ' data component to encapsulate all data functionality.
      '
      '****************************************************************

      Private Sub DeleteBtn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles deleteButton.Click

         ' Only attempt to delete the item if it is an existing item
         ' (new items will have "ItemId" of 0)
         If itemId <> 0 Then

            Dim booksDB As New ASPNetPortal.BooksDB()
            booksDB.DeleteBook(itemId)

         End If

         ' Redirect back to the portal home page
         Response.Redirect(CType(ViewState("UrlReferrer"), String))

      End Sub


      '****************************************************************
      '
      ' The CancelBtn_Click event handler on this Page is used to cancel
      ' out of the page, and return the user back to the portal home
      ' page.
      '
      '****************************************************************

      Private Sub CancelBtn_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cancelButton.Click

         ' Redirect back to the portal home page
         Response.Redirect(CType(ViewState("UrlReferrer"), String))

      End Sub

      Private Sub updateGlobalButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles updateGlobalButton.Click

         ' Update settings in the database
         Dim admin As New AdminDB()

         ' Update columns setting
         admin.UpdateModuleSetting(moduleId, "columns", ColumnsField.Text)
      End Sub
   End Class

End Namespace