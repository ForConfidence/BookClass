﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

public partial class Admin_AccountManagement : System.Web.UI.Page
{
    protected static PagedDataSource pds = new PagedDataSource(); //分页第一步，声明此对象，必须static
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckLogin();
        if (!IsPostBack)
        {
            DataList1Bind(0);
            ArrayList type = new ArrayList();
            type.Add("用  户");
            type.Add("管理员");
        }
    }
    public void CheckLogin() //以下代码检测用户登录参数是否正确
    {

        String StuId = (string)Session["StuId"];
        DataSet dt = SqlHelper.ExecuteDataset(CommandType.Text, "SELECT * FROM [BookClass].[dbo].[UserInfo] WHERE StuId ='" + StuId + "'AND Type = 'manager'");
        if (dt.Tables[0].Rows.Count == 0)
        {
            Session.Abandon();
            Console.Write("您的参数有误，请尝试重新登录。");
            //  System.Threading.Thread.Sleep(10000); 
            Response.Redirect("../login.aspx");
            return;
        }
        else
        {
            DataSet dt1 = SqlHelper.ExecuteDataset(CommandType.Text, "SELECT * FROM [BookClass].[dbo].[UserInfo] WHERE StuId ='" + StuId + "'");
            label_stuNum.Text = dt1.Tables[0].Rows[0][1].ToString();
            label_user.Text = dt1.Tables[0].Rows[0][3].ToString();
        }
    }
    protected void DataList1Bind(int currentpage)
    {
        pds.AllowPaging = true;    //分页第二步
        pds.PageSize = 15;
        pds.CurrentPageIndex = currentpage;
        DataSet DsUserInfo = SqlHelper.ExecuteDataset(CommandType.Text, "SELECT * FROM [BookClass].[dbo].[UserInfo]")
    ;
        pds.DataSource = DsUserInfo.Tables[0].DefaultView;   //绑定该页的数据源，DefaultView
        DataList1.DataSource = pds;
        DataList1.DataSourceID = null;
        DataList1.DataBind();
    }
  
    protected void DataList1_EditCommand(object source, DataListCommandEventArgs e)
    {
        DataList1.EditItemIndex = e.Item.ItemIndex;  //e.Item表示DataList中发生事件的那一项  
        this.DataList1Bind(pds.CurrentPageIndex);  
    }
    protected void DataList1_CancelCommand(object source, DataListCommandEventArgs e)
    {
        DataList1.EditItemIndex = -1;  //当EditItemIndex属性值为-1时，表示不显示EditItemTemplate模板  
        DataList1Bind(pds.CurrentPageIndex);
    }
    protected void DataList1_DeleteCommand(object source, DataListCommandEventArgs e)
    {
        string ID = DataList1.DataKeys[e.Item.ItemIndex].ToString();  
        int JudgeNum1 = SqlHelper.ExecuteNonQuery(CommandType.Text, "DELETE FROM [BookClass].[dbo].[UserInfo] WHERE ID ='" + ID + "'");
        DataList1.EditItemIndex = -1;
        DataList1Bind(pds.CurrentPageIndex);
        if (JudgeNum1> 0)
        {
            Response.Write("<script>alert('删除成功！')</script>");
            return;
        }
        else
        {
            Response.Write("<script>alert('删除失败！')</script>");
            return;
        }     
    }
    protected void DataList1_UpdateCommand(object source, DataListCommandEventArgs e)
    {
        string ID = DataList1.DataKeys[e.Item.ItemIndex].ToString();
        string Name = ((TextBox)e.Item.FindControl("TxtName")).Text;
        string StuId = ((TextBox)e.Item.FindControl("TxtStuId")).Text;
        string Type = ((DropDownList)e.Item.FindControl("DropType")).SelectedValue;
        int JudgeNum = SqlHelper.ExecuteNonQuery(CommandType.Text, "UPDATE [BookClass].[dbo].[UserInfo] SET StuId='"+StuId+"',Type = '"+Type+"',UserName='"+Name+"' WHERE ID ='"+ID+"'");
        DataList1.EditItemIndex = -1;
        DataList1Bind(pds.CurrentPageIndex);
        if (JudgeNum > 0)
        {
            Response.Write("<script>alert('修改成功！')</script>");
            return;
        }
        else
        {
            Response.Write("<script>alert('修改失败！')</script>");
            return;
        }
    }
    protected void DataList1_ItemCommand(object source, DataListCommandEventArgs e)
    {
        //分页第三步
          switch (e.CommandName)
        { 
            case "first":
                pds.CurrentPageIndex = 0;
                DataList1Bind(pds.CurrentPageIndex);
                break;
            case "pre":
                pds.CurrentPageIndex = pds.CurrentPageIndex - 1;
                DataList1Bind(pds.CurrentPageIndex);
                break;
            case "next":
                pds.CurrentPageIndex=pds.CurrentPageIndex + 1;
                DataList1Bind(pds.CurrentPageIndex);
                break;
            case "last":
                pds.CurrentPageIndex = pds.PageCount-1;
                DataList1Bind(pds.CurrentPageIndex);
                break;
            case "search":
                if (e.Item.ItemType == ListItemType.Footer)
                {
                    int PageCount = int.Parse(pds.PageCount.ToString());
                    TextBox txtPage = e.Item.FindControl("txtPage") as TextBox;
                    int MyPageNum = 0;
                    if (!txtPage.Text.Equals(""))
                        MyPageNum = Convert.ToInt32(txtPage.Text.ToString());
                    if(MyPageNum<=0 || MyPageNum >PageCount) 
                        Response.Write("<script>alert('请输入页数并确认没有超出总页数！</script>");
                    else
                        DataList1Bind(MyPageNum - 1);
                }
                break;
        }

    }
    protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        //分页第四步

        if (e.Item.ItemType == ListItemType.Footer)
        {
            Label CurrentPage = e.Item.FindControl("LabCurrentPage") as Label;
            Label PageCount = e.Item.FindControl("LabPageCount") as Label;
            LinkButton FirstPage = e.Item.FindControl("LnkbtnFirst") as LinkButton;
            LinkButton PrePage = e.Item.FindControl("LnkbtnFront") as LinkButton;
            LinkButton NextPage = e.Item.FindControl("LnkbtnNext") as LinkButton;
            LinkButton LastPage = e.Item.FindControl("LnkbtnLast") as LinkButton;
            CurrentPage.Text = Convert.ToString(pds.CurrentPageIndex + 1);
            PageCount.Text = pds.PageCount.ToString();
            if (pds.IsFirstPage)
            {
                FirstPage.Enabled = false;
                PrePage.Enabled = false;
            }
            if (pds.IsLastPage)
            {
                NextPage.Enabled = false;
                LastPage.Enabled = false;
            }
        }
    }
}