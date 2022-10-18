import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import Patients from "./components/Patients";
import Doctors from "./components/Doctors";
import Visits from "./components/Visits";
import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import Home from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/patienturl',
    element: <Patients />
  },
  {
    path: '/doctorurl',
    element: <Doctors />
  },
  {
	path: '/visiturl',
    element: <Visits />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    requireAuth: true,
    element: <FetchData />
  },
   
  ...ApiAuthorzationRoutes
];

export default AppRoutes;
