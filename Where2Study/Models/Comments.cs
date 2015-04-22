using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Where2Study.Models
{
    public class Comments
    {
        public string username;
        public string timestamp;
        public string comment;

        public IEnumerable getComments(int fakultet)
        {
            var db = new Where2Study.Models.w2sDataContext();
            var userdb = new Where2Study.Models.UsersContext();

            var querry = (from fc in db.fakultet_komentaris
                          from usr in userdb.UserProfiles
                          where fc.id_fakultet == fakultet && fc.id_user == usr.UserId
                          select new Where2Study.Models.Comments
                          {
                              username = usr.UserName,
                              timestamp = fc.vrijeme.ToString(),
                              comment = fc.komentar,
                          }
                ).ToList();
            return querry;
        }
    }
}