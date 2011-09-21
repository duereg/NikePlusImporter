using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Import.NikePlus
{
    public sealed class Configuration
    { 
        /** Url to authenticate the user on the NikePlus website */
        public const String AUTHENTICATION_URL = "https://secure-nikerunning.nike.com/nikeplus/v1/services/widget/generate_pin.jsp?login={0}&password={1}";
    
        /** Url to get the list of the available workouts for the authenticated user */
        public const String GET_WORKOUTS_LIST_URL = "https://secure-nikerunning.nike.com/nikeplus/v1/services/app/run_list.jsp";
    
        /** Url to get the details about a specific workout */
        public const String GET_WORKOUT_DETAILS_URL = "https://secure-nikerunning.nike.com/nikeplus/v1/services/app/get_run.jsp?id={0}";
    }
}
