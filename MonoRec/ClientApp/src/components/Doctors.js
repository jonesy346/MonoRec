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

                await populateDoctors(currentUser, role);
            }
            setLoading(false);
        };

        initialize();
    }, []);

    const incrementDoctorId = () => {
        setDoctorId(DoctorId + 1);
    }

    const populateDoctors = async (currentUser, role) => {
        console.log("Populate Doctors function loaded");
        console.log("Current user:", currentUser);
        console.log("User role:", role);
        try {
            const token = await authService.getAccessToken();
            console.log("Access token:", token ? "Token exists (length: " + token.length + ")" : "No token");
            let response;

            if (role === 'Patient') {
                // Fetch patient entity to get patientId
                console.log("Fetching all patients...");
                const headers = !token ? {} : { 'Authorization': `Bearer ${token}` };
                console.log("Request headers:", headers);
                const patientResponse = await fetch('patient', {
                    headers: headers,
                    credentials: 'include'
                });

                if (!patientResponse.ok) {
                    console.error("Failed to fetch patients:", patientResponse.status, patientResponse.statusText);
                    setDoctors([]);
                    return;
                }

                const patientsText = await patientResponse.text();
                console.log("Raw patients response:", patientsText);

                let patients;
                try {
                    patients = JSON.parse(patientsText);
                    console.log("All patients:", patients);
                    console.log("Looking for userId:", currentUser.sub);
                } catch (e) {
                    console.error("Failed to parse patients JSON:", e);
                    console.error("Response text was:", patientsText);
                    setDoctors([]);
                    return;
                }

                // Find the patient entity linked to this user
                const currentPatient = patients.find(p => p.userId === currentUser.sub);
                console.log("Found patient:", currentPatient);

                if (currentPatient) {
                    // Fetch doctors for this patient
                    console.log(`Fetching doctors for patient ${currentPatient.patientId}...`);
                    response = await fetch(`patient/${currentPatient.patientId}/doctor`, {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (!response.ok) {
                        console.error("Failed to fetch doctors:", response.status, response.statusText);
                        const errorText = await response.text();
                        console.error("Error response:", errorText);
                        setDoctors([]);
                        return;
                    }
                } else {
                    console.log("No patient entity found for user");
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
                const responseText = await response.text();
                console.log("Raw doctors response:", responseText);

                try {
                    const data = JSON.parse(responseText);
                    console.log("Parsed doctors data:", data);
                    setDoctors(data);
                } catch (e) {
                    console.error("Failed to parse doctors JSON:", e);
                    console.error("Response text was:", responseText);
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
                <td>{val.doctorId}</td>
                <td>{val.doctorName}</td>
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
                            <th>Id</th>
                            <th>Name</th>
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
