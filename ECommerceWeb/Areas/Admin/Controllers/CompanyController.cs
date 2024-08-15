using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var lstCompany=_unitOfWork.Company.GetAll();
            return View(lstCompany);
        }
        [HttpGet]
        public IActionResult Upsert(int? companyId)
        {
            if (companyId == null || companyId==0) {
                return View(new Company());
            }
            else
            {
                var companyDetail=_unitOfWork.Company.GetFirstorDefault(x=>x.Id==companyId);
                if (companyDetail == null) { return View(); }
                return View(companyDetail);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if(ModelState.IsValid)
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
            var deleteDetails = _unitOfWork.Company.GetFirstorDefault(x => x.Id == companyId);
            if(deleteDetails != null)
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
        #endregion
    }
}
