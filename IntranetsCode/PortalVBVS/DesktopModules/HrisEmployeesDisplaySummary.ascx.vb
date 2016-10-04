Imports System.IO
Imports System.Configuration
Imports ASPNetPortal

Namespace ASPNetPortal.Hris

    Public MustInherit Class HrisEmployeesDisplaySummary
        Inherits ASPNetPortal.PortalModuleControl
        Protected WithEvents P1 As System.Web.UI.HtmlControls.HtmlGenericControl
        Protected WithEvents Label2 As System.Web.UI.WebControls.Label

        Protected WithEvents myDataGrid As System.Web.UI.WebControls.DataGrid
        Protected WithEvents dlHandbookFiles As System.Web.UI.WebControls.DataList

        Private DocPath As String
        Protected WithEvents lblNewEmployees As System.Web.UI.WebControls.Label
        Private Months As Integer

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

        'the ID of the Instance to Display
        Private InstanceID As Integer

        'property to provide access to the InstanceID
        Public Property Instance() As Integer
            Get
                Return InstanceID
            End Get
            Set(ByVal Value As Integer)
                InstanceID = Value
            End Set
        End Property

        '*******************************************************
        '
        ' The Page_Load event handler on this User Control is used to
        ' obtain a DataReader of Site information for the Hris Component
        '
        '*******************************************************'

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            ' Obtain Employee information
            ' and bind to the DataGrid Control
            Months = CInt(ConfigurationSettings.AppSettings("HRISNewEmployeeMonths"))
            If Not Page.IsPostBack Then
                DocPath = ConfigurationSettings.AppSettings("HRISEmployeeHandbookPath")
                Dim dInfo As DirectoryInfo = New DirectoryInfo(DocPath)

                dlHandbookFiles.DataSource = dInfo.GetFiles("*.*")
                dlHandbookFiles.DataBind()

            End If
            BindGrid()
        End Sub

        'Common routine to bind the datagrid to the Employees dataset
        Private Sub BindGrid()
            Dim dv As New DataView()
            dv = EmployeesBiz.GetNewEmployees(Months).Tables(0).DefaultView
            myDataGrid.DataSource = dv

            If dv.Count = 0 Then
                myDataGrid.Visible = False
                lblNewEmployees.Text = "No new Employees employed in the last " & CStr(Months) & " month(s)"
            End If

            'Set the Sort criteria, otherwise it is lost across the paging 
            If ViewState("SortExpression") <> Nothing Then
                dv.Sort = ViewState("SortExpression").ToString()
            End If

            myDataGrid.DataBind()
        End Sub

        Public Sub Employees_PageIndexChanged(ByVal sender As System.Object, ByVal e As DataGridPageChangedEventArgs) Handles myDataGrid.PageIndexChanged
            myDataGrid.CurrentPageIndex = e.NewPageIndex
            BindGrid()
        End Sub

        Public Sub Employees_ItemCreated(ByVal sender As System.Object, ByVal e As DataGridItemEventArgs) Handles myDataGrid.ItemCreated
            Dim lit As System.Web.UI.WebControls.ListItemType = e.Item.ItemType
            If lit = ListItemType.Header Then
                Dim SortNameDDL As New DropDownList()
                SortNameDDL.CssClass = "Normal"
                SortNameDDL.ID = "SortNameDDL"
                Dim li1 As New ListItem("Title", "EmpTitle")
                SortNameDDL.Items.Add(li1)

                Dim li2 As New ListItem("Last Name", "LastName")
                SortNameDDL.Items.Add(li2)

                Dim li3 As New ListItem("First Name", "FirstName")
                SortNameDDL.Items.Add(li3)

                If ViewState("SortFieldIndex") = Nothing Then
                    SortNameDDL.SelectedIndex = CInt(myDataGrid.Attributes("SortFieldIndex"))
                Else
                    SortNameDDL.SelectedIndex = CInt(ViewState("SortFieldIndex"))
                End If



                'Add the Drop down list to the Header cell
                Dim cell As New TableCell()
                cell = e.Item.Controls(0)
                cell.Controls.Add(SortNameDDL)


            End If


        End Sub

        Public Sub Employees_SortCommand(ByVal sender As System.Object, ByVal e As DataGridSortCommandEventArgs) Handles myDataGrid.SortCommand

            Dim dv As New DataView()
            dv = EmployeesBiz.GetNewEmployees(Months).Tables(0).DefaultView
            myDataGrid.DataSource = dv

            If e.SortExpression <> "*" Then
                dv.Sort = e.SortExpression

            Else
                Dim dgItem As DataGridItem = CType(e.CommandSource, DataGridItem)
                Dim ddl1 As DropDownList = CType(dgItem.FindControl("SortNameDDL"), DropDownList)

                ' Get the selected sort expression from the list
                dv.Sort = ddl1.SelectedItem.Value
                ViewState("SortExpression") = ddl1.SelectedItem.Value

                ' Store the currently select sort expression from the dropdownlist
                myDataGrid.Attributes("SortFieldIndex") = CStr(ddl1.SelectedIndex)
                ViewState("SortFieldIndex") = ddl1.SelectedIndex

            End If

            myDataGrid.DataBind()

        End Sub

        '*******************************************************
        '
        ' GetBrowsePathDetails() is a helper method used to create the url   
        ' to view the details of an entity
        '
        ' This method is used in the databinding expression for
        ' the browse Hyperlink within the DataGrid, and is called 
        ' for each row when DataGrid.DataBind() is called.  It is 
        ' defined as a helper method here (as opposed to inline 
        ' within the template) to improve code organization and
        ' avoid embedding logic within the content template.
        '
        '*******************************************************'
        Function GetBrowsePathDetails(ByVal Entity As String, ByVal ID As Integer) As String
            ' if there is content in the database, create an 
            ' url to browse it
            Return EmployeesBiz.GetBrowsePathDetails(Entity, ID)
        End Function
    End Class

End Namespace