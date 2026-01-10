import React, { useState, useEffect } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { LoginMenu } from './api-authorization/LoginMenu';
import authService from './api-authorization/AuthorizeService';
import './NavMenu.css';

export function NavMenu() {
  const [collapsed, setCollapsed] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [userRole, setUserRole] = useState(null);

  useEffect(() => {
    const checkAuth = async () => {
      const authenticated = await authService.isAuthenticated();
      setIsAuthenticated(authenticated);

      if (authenticated) {
        const user = await authService.getUser();
        const role = user?.role || null;
        setUserRole(role);
      } else {
        setUserRole(null);
      }
    };

    checkAuth();

    // Subscribe to authentication changes
    const subscription = authService.subscribe(() => checkAuth());

    return () => {
      authService.unsubscribe(subscription);
    };
  }, []);

  const toggleNavbar = () => {
    setCollapsed(!collapsed);
  };

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" container light>
        <NavbarBrand tag={Link} to="/">MonoRec</NavbarBrand>
        <NavbarToggler onClick={toggleNavbar} className="mr-2" />
        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
          <ul className="navbar-nav flex-grow">
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
            </NavItem>
            {isAuthenticated && (
              <>
                {userRole !== 'Patient' && (
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/patienturl">Patients</NavLink>
                  </NavItem>
                )}
                {userRole !== 'Doctor' && (
                  <NavItem>
                    <NavLink tag={Link} className="text-dark" to="/doctorurl">Doctors</NavLink>
                  </NavItem>
                )}
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/visiturl">Visits</NavLink>
                </NavItem>
              </>
            )}
            <LoginMenu>
            </LoginMenu>
          </ul>
        </Collapse>
      </Navbar>
    </header>
  );
}
