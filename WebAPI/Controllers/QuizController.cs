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
            using (DBLearningModel db = new DBLearningModel())
            {
                //retourneer alle koloms van de tabel behalve de laatste kolom (answer)
                //dit doet die random. Nog wijzigen naar een bepaalde volgorde
                var Qns = db.Question
                    .Select(x => new { QnID = x.QnID, Qn = x.Qn, ImageName = x.ImageName, x.Option1, x.Option2, x.Option3 })
                    .OrderBy(y => Guid.NewGuid())
                    .ToArray();
                //format questions array
                var updated = Qns.AsEnumerable()
                    .Select(x => new
                    {
                        QnID = x.QnID,
                        Qn = x.Qn,
                        ImageName = x.ImageName,
                        Options = new string[] { x.Option1, x.Option2, x.Option3 }
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
            using (DBLearningModel db = new DBLearningModel())
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
