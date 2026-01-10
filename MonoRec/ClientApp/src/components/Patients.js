import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

function Patients(props) {

    //const [Date, setDate] = useState('');
    //const [Notes, setNotes] = useState('');
    //const [PatientId, setPatientId] = useState('');
    //const [Patients, setPatients] = useState([]);

    //useEffect(() => {
    //    populatePatients();
    //}, [Patients]);

    //const populatePatients = async () => {
    //    const token = await authService.getAccessToken();
    //    const user = await authService.getUser();
    //    const response = await fetch('patient', {
    //        headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
    //    })
    //    const data = await response.json();
    //    setPatients(data);
    //}

    //let handlePatientChange = (e) => {
    //    setPatientId(e.target.value)
    //}

    //const handleSubmit = async (event) => {
    //    event.preventDefault();
    //    //const token = await authService.getAccessToken();
    //    const response = await fetch('patient',
    //        {
    //            method: 'POST',
    //            body: JSON.stringify({ Date, Notes, PatientId }),
    //            headers: { 'Content-Type': 'application/json' }
    //        }
    //    );
    //    props.history.push("/visits");
    //}

    //return (
    //    <div className="container">
    //        <h1>Create Visit Entry</h1>
    //        <div className="row">
    //            <div className="col-mid-6">
    //                <form id="statsForm" onSubmit={handleSubmit}>
    //                    <div className="form-group">
    //                        <label htmlFor="visitDate">Visit Date</label>
    //                        <input type="date" name="visitDate" className="form-control" onChange={(e) => setDate(e.target.value)} />
    //                    </div>
    //                    <div className="form-group">
    //                        <label htmlFor="notes">Notes</label>
    //                        <input type="text" name="notes" className="form-control" onChange={(e) => setNotes(e.target.value)} />
    //                    </div>
    //                    <select onChange={handlePatientChange}>
    //                        <option value="⬇️ Select a patient ⬇️"> -- Select a patient -- </option>
    //                        {Patients.map((patient) => <option value={patient.patientId}>{patient.patientName} | {patient.patientId}</option>)}
    //                    </select>
    //                    <div className="form-group">
    //                        <button type="submit" className="btn btn-success">Submit</button>
    //                    </div>
    //                </form>
    //            </div>
    //        </div>
    //    </div>
    //);

    const [PatientId, setPatientId] = useState(0);
    const [Patients, setPatients] = useState([]);
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

                await populatePatients(currentUser, role);
            }
            setLoading(false);
        };

        initialize();
    }, []);

    const incrementPatientId = () => {
        setPatientId(PatientId + 1);
    }

    const populatePatients = async (currentUser, role) => {
        console.log("Populate Patients function loaded");
        console.log("Current user:", currentUser);
        console.log("User role:", role);
        try {
            const token = await authService.getAccessToken();
            console.log("Access token:", token ? "Token exists (length: " + token.length + ")" : "No token");
            let response;

            if (role === 'Doctor') {
                // Fetch doctor entity to get doctorId
                console.log("Fetching all doctors...");
                const headers = !token ? {} : { 'Authorization': `Bearer ${token}` };
                console.log("Request headers:", headers);
                const doctorResponse = await fetch('doctor', {
                    headers: headers,
                    credentials: 'include'
                });

                if (!doctorResponse.ok) {
                    console.error("Failed to fetch doctors:", doctorResponse.status, doctorResponse.statusText);
                    setPatients([]);
                    return;
                }

                const doctorsText = await doctorResponse.text();
                console.log("Raw doctors response:", doctorsText);

                let doctors;
                try {
                    doctors = JSON.parse(doctorsText);
                    console.log("All doctors:", doctors);
                    console.log("Looking for userId:", currentUser.sub);
                } catch (e) {
                    console.error("Failed to parse doctors JSON:", e);
                    console.error("Response text was:", doctorsText);
                    setPatients([]);
                    return;
                }

                // Find the doctor entity linked to this user
                const currentDoctor = doctors.find(d => d.userId === currentUser.sub);
                console.log("Found doctor:", currentDoctor);

                if (currentDoctor) {
                    // Fetch patients for this doctor
                    console.log(`Fetching patients for doctor ${currentDoctor.doctorId}...`);
                    response = await fetch(`doctor/${currentDoctor.doctorId}/patient`, {
                        headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                        credentials: 'include'
                    });

                    if (!response.ok) {
                        console.error("Failed to fetch patients:", response.status, response.statusText);
                        const errorText = await response.text();
                        console.error("Error response:", errorText);
                        setPatients([]);
                        return;
                    }
                } else {
                    console.log("No doctor entity found for user");
                    setPatients([]);
                    return;
                }
            } else if (role === 'Patient') {
                // Patients shouldn't see this page, but if they do, show empty
                setPatients([]);
                return;
            } else {
                // For other roles, show all patients
                response = await fetch('patient', {
                    method: 'GET',
                    headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
                    credentials: 'include'
                });
            }

            if (response && response.ok) {
                const responseText = await response.text();
                console.log("Raw patients response:", responseText);

                try {
                    const data = JSON.parse(responseText);
                    console.log("Parsed patients data:", data);
                    setPatients(data);
                } catch (e) {
                    console.error("Failed to parse patients JSON:", e);
                    console.error("Response text was:", responseText);
                    setPatients([]);
                }
            }
        } catch (error) {
            console.error("Error fetching patients:", error);
        }
    }

    const tableRows = Patients.map((val, key) => {
        return (
            <tr key={key}>
                <td>{val.patientName}</td>
                <td>{val.patientEmail}</td>
                <td><button className="btn btn-primary" onClick={incrementPatientId}>Edit</button></td>
            </tr>
        );
    });

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return (
            <div className="Patients">
                <h1>Patients</h1>
                <p>Please log in to view patients.</p>
            </div>
        );
    }

    return (
        <div className="Patients">
            <h1>Patients</h1>

            {userRole === 'Doctor' && Patients.length === 0 && (
                <p>No patients assigned yet. Patients will be automatically assigned when you first log in.</p>
            )}

            {userRole === 'Patient' && (
                <p>This page is for doctors only.</p>
            )}

            {Patients.length > 0 && (
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

export default Patients;