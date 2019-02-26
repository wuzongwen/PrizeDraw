using System.Web;
using System.Web.Optimization;

namespace PrizeDraw.App_Start
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //layui
            bundles.Add(new ScriptBundle("~/Content/layui/js").Include(
                "~/Content/layui/layui.js"));
            bundles.Add(new StyleBundle("~/Content/layui.css").Include(
                "~/Content/layui/css/layui.css"));

            //jquery
            bundles.Add(new ScriptBundle("~/Content/jquery/js").Include(
               "~/Content/Jquery/1.11.3/jquery.min.js"));

            //PagedList.css
            bundles.Add(new StyleBundle("~/Content/PagedList/css").Include(
                        "~/Content/PagedList/PagedList.css"));

            //wangEditor.min.js
            bundles.Add(new ScriptBundle("~/Content/wangEditor/js").Include(
                "~/Content/wangEditor/wangEditor.min.js"));

            //clipboard.min.js
            bundles.Add(new ScriptBundle("~/Content/Clipboard/js").Include(
                        "~/Content/Clipboard/clipboard.min.js"));

            //login.js
            bundles.Add(new ScriptBundle("~/Admin/login/js").Include(
                        "~/Content/Admin/static/js/login.js"));

            #region User
            bundles.Add(new ScriptBundle("~/User/list/js").Include(
                        "~/Content/Admin/static/js/User/list.js"));
            bundles.Add(new ScriptBundle("~/User/edit/js").Include(
                        "~/Content/Admin/static/js/User/edit.js"));
            bundles.Add(new ScriptBundle("~/User/add/js").Include(
                        "~/Content/Admin/static/js/User/add.js"));
            #endregion

            #region Library
            bundles.Add(new ScriptBundle("~/Library/list/js").Include(
                        "~/Content/Admin/static/js/Library/list.js"));
            bundles.Add(new ScriptBundle("~/Library/edit/js").Include(
                        "~/Content/Admin/static/js/Library/edit.js"));
            bundles.Add(new ScriptBundle("~/Library/add/js").Include(
                        "~/Content/Admin/static/js/Library/add.js"));
            bundles.Add(new ScriptBundle("~/Library/libraryuser/js").Include(
                        "~/Content/Admin/static/js/Library/libraryuser.js"));
            #endregion

            #region Content
            bundles.Add(new ScriptBundle("~/Content/list/js").Include(
                        "~/Content/Admin/static/js/Content/contentlist.js"));
            bundles.Add(new ScriptBundle("~/Content/newbooklist/js").Include(
                        "~/Content/Admin/static/js/Content/newbooklist.js"));
            bundles.Add(new ScriptBundle("~/Content/newbook/js").Include(
                        "~/Content/Admin/static/js/Content/newbook.js"));
            bundles.Add(new ScriptBundle("~/Content/notice/js").Include(
                        "~/Content/Admin/static/js/Content/notice.js"));
            bundles.Add(new ScriptBundle("~/Content/slide/js").Include(
                        "~/Content/Admin/static/js/Content/slide.js"));
            bundles.Add(new ScriptBundle("~/Content/slideimgedit/js").Include(
                        "~/Content/Admin/static/js/Content/slideimgedit.js"));
            bundles.Add(new ScriptBundle("~/Content/video/js").Include(
                        "~/Content/Admin/static/js/Content/video.js"));
            #endregion

            #region Other
            bundles.Add(new ScriptBundle("~/Other/additionaldatalist/js").Include(
                        "~/Content/Admin/static/js/Other/additionaldatalist.js"));
            bundles.Add(new ScriptBundle("~/Other/additionaldata/js").Include(
                        "~/Content/Admin/static/js/Other/additionaldata.js"));
            bundles.Add(new ScriptBundle("~/Other/custdevicelist/js").Include(
                        "~/Content/Admin/static/js/Other/custdevicelist.js"));
            bundles.Add(new ScriptBundle("~/Other/custdevice/js").Include(
                        "~/Content/Admin/static/js/Other/custdevice.js"));
            #endregion
        }
    }
}