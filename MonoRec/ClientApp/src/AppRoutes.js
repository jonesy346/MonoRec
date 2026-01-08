import ApiAuthorzationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import Patients from "./components/Patients";
import Doctors from "./components/Doctors";
import Visits from "./components/Visits";
import Home from "./components/Home";
import { AuthHealthCheck } from "./components/AuthHealthCheck";

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
    path: '/auth-health-check',
    element: <AuthHealthCheck />
  },

  ...ApiAuthorzationRoutes
];

export default AppRoutes;
