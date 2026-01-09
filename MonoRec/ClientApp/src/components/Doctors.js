import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

function Doctors(props) {

    const [DoctorId, setDoctorId] = useState(0);
    const [Doctors, setDoctors] = useState([]);
    const [user, setUser] = useState(null);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const initialize = async () => {
            const authenticated = await authService.isAuthenticated();
            setIsAuthenticated(authenticated);

            if (authenticated) {
                const currentUser = await authService.getUser();
                setUser(currentUser);
                const role = currentUser.role || 'User';
                setUserRole(role);

                await populateDoctors(role);
            }
            setLoading(false);
        };

        initialize();
    }, []);

    const incrementDoctorId = () => {
        setDoctorId(DoctorId + 1);
    }

    // Security implementation: Patients fetch their own patient record using currentUserOnly=true query parameter,
    // which ensures the backend only returns their own patient entity. This prevents exposure of other patients' data
    // while still allowing patients to find their patientId and retrieve their associated doctors.
    const populateDoctors = async (role) => {
        try {
            const token = await authService.getAccessToken();
            let response;

            if (role === 'Patient') {
                // Fetch current patient entity using backend filtering (currentUserOnly=true)
                const headers = !token ? {} : { 'Authorization': `Bearer ${token}` };
                const patientResponse = await fetch('patient?currentUserOnly=true', {
                    headers: headers,
                    credentials: 'include'
                });

                if (!patientResponse.ok) {
                    console.error("Failed to fetch current patient:", patientResponse.status, patientResponse.statusText);
                    setDoctors([]);
                    return;
                }

                let patients;
                try {
                    patients = await patientResponse.json();
                } catch (e) {
                    console.error("Failed to parse patients JSON:", e);
                    setDoctors([]);
                    return;
                }

                // Should only have one patient (the current user)
                const currentPatient = patients[0];

                if (currentPatient) {
                    // Fetch doctors for this patient
                    response = await fetch(`patient/${currentPatient.patientId}/doctor`, {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (!response.ok) {
                        console.error("Failed to fetch doctors:", response.status, response.statusText);
                        setDoctors([]);
                        return;
                    }
                } else {
                    setDoctors([]);
                    return;
                }
            } else if (role === 'Doctor') {
                // Doctors shouldn't see this page, but if they do, show empty
                setDoctors([]);
                return;
            } else {
                // For other roles, show all doctors
                response = await fetch('doctor', {
                    method: 'GET',
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });
            }

            if (response && response.ok) {
                try {
                    const data = await response.json();
                    setDoctors(data);
                } catch (e) {
                    console.error("Failed to parse doctors JSON:", e);
                    setDoctors([]);
                }
            }
        } catch (error) {
            console.error("Error fetching doctors:", error);
        }
    }

    const tableRows = Doctors.map((val, key) => {
        return (
            <tr key={key}>
                <td>{val.doctorName}</td>
                <td>{val.doctorEmail}</td>
                <td><button className="btn btn-primary" onClick={incrementDoctorId}>Edit</button></td>
            </tr>
        );
    });

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return (
            <div className="Doctors">
                <h1>Doctors</h1>
                <p>Please log in to view doctors.</p>
            </div>
        );
    }

    return (
        <div className="Doctors">
            <h1>Doctors</h1>

            {userRole === 'Patient' && Doctors.length === 0 && (
                <p>No doctors assigned yet. Doctors will be automatically assigned when you first log in.</p>
            )}

            {userRole === 'Doctor' && (
                <p>This page is for patients only.</p>
            )}

            {Doctors.length > 0 && (
                <table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Button</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableRows}
                    </tbody>
                </table>
            )}
        </div>
    );
}

export default Doctors;
