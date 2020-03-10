using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class QuizController : ApiController
    {
        //Controller om 10 vragen te retourneren
        [HttpGet]
        [Route("api/Questions")]
        public HttpResponseMessage GetQuestions()
        {
            using (DBModel db = new DBModel())
            {
                //retourneer alle koloms van de tabel behalve de laatste kolom (answer)
                //dit doet die random. Nog wijzigen naar een bepaalde volgorde
                var Qns = db.Question
                    .Select(x => new { QnID = x.QnID, Qn = x.Qn, ImageName = x.ImageName, x.Option1, x.Option2,
                        x.BUS1, x.BUS2, x.SOF1, x.SOF2, x.CS1, x.CS2, x.IOT1, x.IOT2 })
                    .OrderBy(y => Guid.NewGuid())
                    .ToArray();
                //format questions array
                var updated = Qns.AsEnumerable()
                    .Select(x => new
                    {
                        QnID = x.QnID,
                        Qn = x.Qn,
                        BUS1 = x.BUS1,
                        BUS2 = x.BUS2,
                        SOF1 = x.SOF1,
                        SOF2 = x.SOF2,
                        CS1 = x.CS1,
                        CS2 = x.CS2,
                        IOT1 = x.IOT1,
                        IOT2 = x.IOT2,
                        Options = new string[] { x.Option1, x.Option2 }
                    }).ToList();
                //then return the questions
                return this.Request.CreateResponse(HttpStatusCode.OK, updated);
            }
        }

        //to return the answers
        [HttpPost]
        [Route("api/Answers")]
        public HttpResponseMessage GetAnswers(int[] qIDs)
        {
            using (DBModel db = new DBModel())
            {
                var result = db.Question
                     .AsEnumerable()
                     .Where(y => qIDs.Contains(y.QnID))
                     .OrderBy(x => { return Array.IndexOf(qIDs, x.QnID); })
                     .Select(z => z.Answer)
                     .ToArray();
                return this.Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }
    }
}
