using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Adventureworks.Domain;
using Adventureworks.FileRepository.Common;
using Adventureworks.Web.PurchasingService;
using Adventureworks.FileRepository;

namespace Adventureworks.Web.Controllers
{
    public class PurchasingController : Controller
    {

        //
        // GET: /Purchasing/

        public ActionResult Index()
        {
            IEnumerable<RequestForProposal> activeRfps = RetrieveActive();
            return View(activeRfps);
        }

        private IEnumerable<RequestForProposal> RetrieveActive()
        {
            //  if no persistence file, exit
            if (!System.IO.File.Exists(IOHelper.GetAllRfpsFileName()))
                return new List<RequestForProposal>();

            // load the document
            XElement doc = XElement.Load(IOHelper.GetAllRfpsFileName());

            // fetch active rfps
            return
                from rfp in doc.Descendants("requestForProposal")
                where (rfp.Attribute("status").Value.Equals("active"))
                select MapFrom(rfp);   
        }

        // map a Request for Proposal from a Linq to Xml XElement
        public RequestForProposal MapFrom(XElement elem)
        {
            RequestForProposal rfp = new RequestForProposal();

            rfp.ID = new Guid(elem.Attribute("id").Value);
            rfp.Status = elem.Attribute("status").Value;
            rfp.Title = elem.Element("title").Value;
            rfp.Description = elem.Element("description").Value;
            rfp.CreationDate = DateTime.Parse(elem.Element("creationDate").Value, new CultureInfo("EN-us"));

            if (elem.Element("completionDate") != null)
                rfp.CompletionDate = DateTime.Parse(elem.Element("completionDate").Value, new CultureInfo("EN-us"));

            // invited vendors
            foreach (XElement vendorElem in elem.Element("invitedVendors").Elements("vendor"))
            {
                Vendor vendor = VendorRepository.Retrieve(Convert.ToInt32(vendorElem.Attribute("id").Value, new CultureInfo("EN-us")));
                rfp.InvitedVendors.Add(vendor);
            }

            // map received proposals in the list
            foreach (var proposal in elem.Element("vendorProposals").Elements("vendorProposal"))
            {
                Vendor vendor = VendorRepository.Retrieve(int.Parse(proposal.Attribute("vendorId").Value, new CultureInfo("EN-us")));
                VendorProposal vendorProposal = new VendorProposal(vendor.Id);
                vendorProposal.Value = double.Parse(proposal.Attribute("value").Value, new CultureInfo("EN-us"));
                vendorProposal.Date = DateTime.Parse(proposal.Attribute("date").Value, new CultureInfo("EN-us"));
                rfp.VendorProposals.Add(vendor.Id, vendorProposal);
            }

            // map best proposal
            if (elem.Element("bestProposal") != null)
            {
                Vendor bestVendor = VendorRepository.Retrieve(Convert.ToInt32(elem.Element("bestProposal").Attribute("vendorId").Value, new CultureInfo("EN-us")));
                rfp.BestProposal = new VendorProposal(bestVendor.Id);
                rfp.BestProposal.Value = double.Parse(elem.Element("bestProposal").Attribute("value").Value, new CultureInfo("EN-us"));
                rfp.BestProposal.Date = DateTime.Parse(elem.Element("bestProposal").Attribute("date").Value, new CultureInfo("EN-us"));
            }

            return rfp;
        }

        public ActionResult CreateRfp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRfp(RequestForProposal rfp)
        {
            IPurchasingService purchasingService = new PurchasingServiceClient();
            rfp.Status = "active";
            string[] invitedVendors = Request.Form["InvitedVendors"].Split(',');
            foreach (var invitedVendor in invitedVendors)
            {
                rfp.InvitedVendors.Add(VendorRepository.Retrieve(int.Parse(invitedVendor)));
            }

            try
            {
                SubmitPurchasingProposalResponse response = purchasingService.SubmitPurchasingProposal(new SubmitPurchasingProposalRequest(rfp));
            }
            catch (Exception)
            {
                
                throw;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult VendorProposal(VendorSubmitProposalRequest svp)
        {
            IPurchasingService purchasingService = new PurchasingServiceClient();

            try
            {
                purchasingService.SubmitVendorProposal(new SubmitVendorProposalRequest(svp));
            }
            catch (Exception)
            {
                
                throw;
            }

            return RedirectToAction("Index");
        }

        public ActionResult VendorProposal(Guid id, int vendorId)
        {
            RequestForProposal rfp = Retrieve(id);
            ViewBag.VendorId = vendorId;

            return View(rfp);
        }

        private RequestForProposal Retrieve(Guid id)
        {
            // load the document
            XElement doc = XElement.Load(IOHelper.GetAllRfpsFileName());

            // erase nodes for the current rfp
            IEnumerable<RequestForProposal> current =
                                    from r in doc.Elements("requestForProposal")
                                    where r.Attribute("id").Value.Equals(id.ToString())
                                    select MapFrom(r);

            return current.First<RequestForProposal>();
        }

        public ActionResult FinishedProposals()
        {
            IEnumerable<RequestForProposal> finishedRfps = RetrieveFinished();

            return View(finishedRfps);
        }

        private IEnumerable<RequestForProposal> RetrieveFinished()
        {
            //  if no persistence file, exit
            if (!System.IO.File.Exists(IOHelper.GetAllRfpsFileName()))
                return new List<RequestForProposal>();

            // load the document
            XElement doc = XElement.Load(IOHelper.GetAllRfpsFileName());

            // fetch active rfps
            return
                from rfp in doc.Descendants("requestForProposal")
                where (rfp.Attribute("status").Value.Equals("finished"))
                select MapFrom(rfp);
        }
    }
}
