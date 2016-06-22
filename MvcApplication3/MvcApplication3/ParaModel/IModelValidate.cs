using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MvcApplication3.ViewModel;

namespace MvcApplication3.ParaModel
{
    interface IModelValidate
    {
        PCResponseModel ModelValidate(ValidationContext validationContext);
    }
}
