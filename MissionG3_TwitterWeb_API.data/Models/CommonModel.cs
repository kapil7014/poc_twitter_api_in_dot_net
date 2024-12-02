using System;
using System.Collections.Generic;
using System.Text;

namespace MissionG3_TwitterWeb_API.data.Models
{
    public class CommonModel<T> where T : class
    {
        public CommonModel(T data)
        {
            this.data = data;
        }

        public T data { get; private set; }

    }
}
