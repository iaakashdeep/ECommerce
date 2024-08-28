using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =StaticDetails.Role_Admin)]
    [ServiceFilter(typeof(BaseExceptionController))]        //Adding custom exception filter
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            try
            {
                var lstCompany = _unitOfWork.Company.GetAll();
                return View(lstCompany);
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }
            
        }
        [HttpGet]
        
        public IActionResult Upsert(int? companyId)
        {
            try
            {
                if (companyId == null || companyId == 0)
                {
                    return View(new Company());
                }
                else
                {
                    var companyDetail = _unitOfWork.Company.GetFirstorDefault(x => x.Id == companyId);
                    if (companyDetail == null) { return View(); }
                    return View(companyDetail);
                }
            }
            
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (company.Id == 0)
                    {
                        _unitOfWork.Company.Add(company);
                    }
                    else
                    {
                        _unitOfWork.Company.Update(company);
                    }
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                return View(company);
            }
            
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        #region API Calls

        [HttpGet]
        public IActionResult GetCompany()
        {
            var lstCompany = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data = lstCompany});
        }
        [HttpDelete]
        public IActionResult Delete(int? companyId)
        {
            try
            {
                var deleteDetails = _unitOfWork.Company.GetFirstorDefault(x => x.Id == companyId);
                if (deleteDetails != null)
                {
                    _unitOfWork.Company.Remove(deleteDetails);
                    _unitOfWork.Save();
                    return Json(new { success = true, message = "Company Deleted successfully!" });
                }
                else
                {
                    return Json(new { success = false, message = "Error in Deletion!" });
                }
            }
            

            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
