using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Nakliyat.Models;
using Nakliyat.ViewModels;
using Nakliyat.Controllers;
namespace Nakliyat.Controllers
{
    public class HomeController : Controller
    {
         nakliyatEntities db = new nakliyatEntities();

        TimeSetCS time = new TimeSetCS();

        public ActionResult Index()
        {

            //SitemapGenerator sitemap = new SitemapGenerator();
          
            //foreach (var item in db.sitemapUrl.ToList())
            //{
            //    sitemap.setFillParametres(item.Yol, item.Lastmod);
            //}

            //sitemap.generate(db.sirketBilgileri.FirstOrDefault().URL_Sitemap_icin, Server.MapPath("~/"));








            var model = new HomeViewModel() { resimlerV = db.Resimler.ToList(), sirketBilgileriV = db.sirketBilgileri.FirstOrDefault() };
            
            return View(model);
           
        }

        public ActionResult Hakkimizda()
        {


            var model = new HakkimizdaViewModel() { pageHakkimizdaV=db.pageHakkimizdaa.FirstOrDefault(), sirketBilgileriV = db.sirketBilgileri.FirstOrDefault() ,ResimlerV = db.Resimler.ToList()};
            return View(model);
        }

        public ActionResult Iletisim(int? basariliMi)
        {
            var model = new IletisimViewModel() { pageIletisimV = db.pageIletisim.FirstOrDefault(), sirketBilgileriV = db.sirketBilgileri.FirstOrDefault() };
            if (basariliMi != null)
                ViewBag.basariliMi = 1;
            return View(model);
        }

        [HttpPost]
        public ActionResult IletisimFormu(iletisimFormu form)
        {
            try
            {

                form.Tarih = time.getTime();
                db.iletisimFormu.Add(form);
                db.SaveChanges();
                return RedirectToAction("Iletisim", new { basariliMi = 1 });
            }
            catch 
            {

                return RedirectToAction("Iletisim", new { basariliMi = 0 });
            }
      
        }

        public ActionResult HizmetBolgeleri()
        {
            var model = new HizmetViewModel() { pagesHizmetEklemeV = db.pagesHizmetEkleme.ToList(), sirketBilgileriV = db.sirketBilgileri.FirstOrDefault() };
            return View(model);
        }
        public ActionResult Depolama(int? fiyat)
        {
            var model = new DepolamaViewModel() { depolamaV = db.depolama.ToList(), sirketBilgileriV = db.sirketBilgileri.FirstOrDefault() };
            if (fiyat != null)
                ViewBag.Fiyat = fiyat;
            return View(model);
        }
        public ActionResult HizmetBolgelerimiz(string name)
        {

            //Girilen hizmet bölgesinin cshtml'ini aratıp buluyoruz
            string newName = "HizmetBolgelerimiz_" + name;
            ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, newName, null);
            // check if view name requested is not found
            if (result == null || result.View == null)
            {
                return new HttpNotFoundResult();
            }
            // otherwise just return the view
            var model = new DepolamaViewModel() { depolamaV = db.depolama.ToList(), sirketBilgileriV = db.sirketBilgileri.FirstOrDefault() };
            return View(newName,model);

           
           
        }
        [HttpPost]
        public ActionResult DepolamaHesapla(int gun,int parkeli,int bodrum,int metrekare)
        {
      
            int parkeF, bodrumF, metrekareF, fiyat;
            if (parkeli==1)
                parkeF = db.depolama.Where(x => x.EtkenAdı == "Parkeli").FirstOrDefault().GunlukEtkisi;
            else
                parkeF = db.depolama.Where(x => x.EtkenAdı == "Parkesiz").FirstOrDefault().GunlukEtkisi;
            if (bodrum == 1)
                bodrumF = db.depolama.Where(x => x.EtkenAdı == "Yüksek Giriş").FirstOrDefault().GunlukEtkisi;
            else
                bodrumF = db.depolama.Where(x => x.EtkenAdı == "Bodrum").FirstOrDefault().GunlukEtkisi;

            metrekareF= db.depolama.Where(x => x.EtkenAdı == "Metrekare").FirstOrDefault().GunlukEtkisi;
            fiyat = gun*(parkeF + bodrumF + metrekare);

            return RedirectToAction("Depolama",new { fiyat = fiyat });
        }

        [HttpPost]
        public ActionResult Login(users user,string action)
        {
            var kllnci = db.users.Where(x => x.kullaniciAdi.ToString() == user.kullaniciAdi.ToString() && x.sifre.ToString() == user.sifre.ToString()).FirstOrDefault();
            if (kllnci == null)
                return RedirectToAction(action, new { yanlisGiris = 1 });

            FormsAuthentication.SetAuthCookie(kllnci.kullaniciAdi, false);
            return RedirectToAction("Index", "Admin/AdminHome");
        }
    }
}