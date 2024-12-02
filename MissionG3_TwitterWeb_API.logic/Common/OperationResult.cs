using System.Collections.Generic;
using System.Linq;

namespace MissionG3_TwitterWeb_API.logic.Common
{
     public class OperationResult
    {
        public OperationResult() { }

        public OperationResult(bool success)
            : this(success, null)
        { }

        public OperationResult(bool success, string errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
    }

    public class OperationResult<T> where T : class
    {
        public OperationResult() { }

        public OperationResult(bool success)
        {
            this.Success = success;
        }

        public OperationResult(bool success, string errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }
        
        public OperationResult(bool success, string errorMessage, T model)
        {
            this.Model = model;
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public OperationResult(bool success, string errorMessage, IEnumerable<T> stateList)
        {
            this.StateList = stateList;
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public T Model { get; private set; }
        public IEnumerable<T> StateList { get; private set; }
    }

    public class DataTableResult<T> where T : class
    {
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public int draw { get; set; }

        public DataTableResult() { }

        public DataTableResult(bool success, string errorMessage)
        {
            this.Success = success;
            this.ErrorMessage = errorMessage;
        }

        public DataTableResult(bool success, string errorMessage, IEnumerable<T> stateList, int draw)
        {
            this.data = stateList;
            this.draw = draw;
        }

        public int recordsTotal {
            get
            {
                if (data == null)
                    return 0;
                return data.Count();
            }
        }

        public int recordsFiltered
        {
            get
            {
                if (data == null)
                    return 0;
                return data.Count();
            }
        }

        public IEnumerable<T> data { get; private set; }
    }
}