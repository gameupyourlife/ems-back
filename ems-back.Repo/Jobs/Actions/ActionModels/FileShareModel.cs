using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ems_back.Repo.Models.Types;

namespace ems_back.Repo.Jobs.Actions.ActionModels
{
    public class FileShareModel
    {
        public ActionType ActionType => ActionType.ShareFile;
        public string FileId { get; set; }
    }
}
