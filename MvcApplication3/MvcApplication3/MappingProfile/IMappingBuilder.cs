using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication3.MappingProfile
{
    interface IMappingBuilder<T>
    {
        void ViewModelBuild(T data);
    }
}
