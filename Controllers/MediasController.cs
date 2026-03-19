using DAL;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;


[UserAccess(Access.View)]
public class MediasController : Controller
{
    private void InitSessionVariables()
    {
        if (Session["CurrentMediaId"] == null) Session["CurrentMediaId"] = 0;
        if (Session["CurrentMediaTitle"] == null) Session["CurrentMediaTitle"] = "";
        if (Session["Search"] == null) Session["Search"] = false;
        if (Session["SearchString"] == null) Session["SearchString"] = "";
        if (Session["SelectedCategory"] == null) Session["SelectedCategory"] = "";
        if (Session["Categories"] == null) Session["Categories"] = DB.Medias.MediasCategories();
        if (Session["SortByTitle"] == null) Session["SortByTitle"] = true;
        if (Session["SortAscending"] == null) Session["SortAscending"] = true;
        ValidateSelectedCategory();
    }

    private void ResetCurrentMediaInfo()
    {
        Session["CurrentMediaId"] = 0;
        Session["CurrentMediaTitle"] = "";
    }

    private void ValidateSelectedCategory()
    {
        if (Session["SelectedCategory"] != null)
        {
            var selectedCategory = (string)Session["SelectedCategory"];
            var medias = DB.Medias.ToList().Where(c => c.Category == selectedCategory);
            if (medias.Count() == 0)
                Session["SelectedCategory"] = "";
        }
    }

    public ActionResult GetMediasCategoriesList(bool forceRefresh = false)
    {
        try
        {
            InitSessionVariables();

            bool search = (bool)Session["Search"];

            if (search)
            {
                return PartialView();
            }
            return null;
        }
        catch (System.Exception ex)
        {
            return Content("Erreur interne " + ex.Message, "text/html");
        }
    }

    public ActionResult GetMedias(bool forceRefresh = false)
    {
        try
        {
            IEnumerable<Media> result = null;

            if (DB.Medias.HasChanged || forceRefresh)
            {
                InitSessionVariables();
                bool search = (bool)Session["Search"];
                string searchString = (string)Session["SearchString"];

                if (search)
                {
                    result = DB.Medias.ToList()
                        .Where(c => c.Title.ToLower().Contains(searchString))
                        .OrderBy(c => c.Title);

                    string selectedCategory = (string)Session["SelectedCategory"];
                    if (selectedCategory != "")
                        result = result.Where(c => c.Category == selectedCategory);
                }
                else
                {
                    result = DB.Medias.ToList();
                }

                if ((bool)Session["SortAscending"])
                {
                    if ((bool)Session["SortByTitle"])
                        result = result.OrderBy(c => c.Title);
                    else
                        result = result.OrderBy(c => c.PublishDate);
                }
                else
                {
                    if ((bool)Session["SortByTitle"])
                        result = result.OrderByDescending(c => c.Title);
                    else
                        result = result.OrderByDescending(c => c.PublishDate);
                }

                return PartialView(result);
            }
            return null;
        }
        catch (System.Exception ex)
        {
            return Content("Erreur interne " + ex.Message, "text/html");
        }
    }

    public ActionResult GetMediaDetails(bool forceRefresh = false)
    {
        int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;

        if (id != 0)
        {
            if (DB.Medias.HasChanged || forceRefresh)
            {
                Media media = DB.Medias.Get(id);
                if (media != null)
                {
                    return PartialView(media);
                }
            }
        }

        return null;
    }
    public ActionResult List()
    {
        ResetCurrentMediaInfo();
        return View();
    }

    public ActionResult ToggleSearch()
    {
        if (Session["Search"] == null) Session["Search"] = false;
        Session["Search"] = !(bool)Session["Search"];
        return RedirectToAction("List");
    }

    public ActionResult SortByTitle()
    {
        Session["SortByTitle"] = true;
        return RedirectToAction("List");
    }

    public ActionResult ToggleSort()
    {
        Session["SortAscending"] = !(bool)Session["SortAscending"];
        return RedirectToAction("List");
    }

    public ActionResult SortByDate()
    {
        Session["SortByTitle"] = false;
        return RedirectToAction("List");
    }

    public ActionResult SetSearchString(string value)
    {
        Session["SearchString"] = value.ToLower();
        return RedirectToAction("List");
    }

    public ActionResult SetSearchCategory(string value)
    {
        Session["SelectedCategory"] = value;
        return RedirectToAction("List");
    }

    public ActionResult About()
    {
        return View();
    }

    public ActionResult Details(int id)
    {
        Session["CurrentMediaId"] = id;
        Media media = DB.Medias.Get(id);
        if (media != null)
        {
            Session["CurrentMediaTitle"] = media.Title;
            return View(media);
        }
        return RedirectToAction("List");
    }

    [UserAccess(Access.Write)]
    public ActionResult Create()
    {
        return View(new Media());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [UserAccess(Access.Write)]
    public ActionResult Create(Media media)
    {
        DB.Medias.Add(media);
        return RedirectToAction("List");
    }

    [UserAccess(Access.Write)]
    public ActionResult Edit()
    {
        int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;
        if (id != 0)
        {
            Media media = DB.Medias.Get(id);
            if (media != null)
                return View(media);
        }
        return RedirectToAction("List");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [UserAccess(Access.Write)]
    public ActionResult Edit(Media media)
    {
        int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;

        Media storedMedia = DB.Medias.Get(id);
        if (storedMedia != null)
        {
            media.Id = id;
            media.PublishDate = storedMedia.PublishDate;
            DB.Medias.Update(media);
        }
        return RedirectToAction("Details/" + id);
    }

    [UserAccess(Access.Write)]
    public ActionResult Delete()
    {
        int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;
        if (id != 0)
        {
            DB.Medias.Delete(id);
        }
        return RedirectToAction("List");
    }
    // This action is meant to be called by an AJAX request
    // Return true if there is a name conflict
    // Look into validation.js for more details
    // and also into Views/Medias/MediaForm.cshtml public JsonResult CheckConflict(string YoutubeId)
    public JsonResult CheckConflict(string YoutubeId)
    {
        int id = Session["CurrentMediaId"] != null ? (int)Session["CurrentMediaId"] : 0;
        return Json(
            DB.Medias.ToList().Where(c => c.YoutubeId == YoutubeId && c.Id != id).Any(),
            JsonRequestBehavior.AllowGet
        );
    }
}