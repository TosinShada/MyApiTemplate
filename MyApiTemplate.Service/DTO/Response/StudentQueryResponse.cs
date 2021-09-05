using System;
using System.Collections.Generic;

namespace MyApiTemplate.Service.DTO.Response
{
    public class StudentQueryResponse
    {
        public DateTime createdAt { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string id { get; set; }
    }
}
