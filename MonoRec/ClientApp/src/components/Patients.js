import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

function Patients(props) {

    const [Date, setDate] = useState('');
    const [Notes, setNotes] = useState('');
    const [PatientId, setPatientId] = useState('');
    const [Patients, setPatients] = useState([]);

    useEffect(() => {
        populatePatients();
    }, [Patients]);

    const populatePatients = async () => {
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        const response = await fetch('patient', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        })
        const data = await response.json();
        setPatients(data);
    }

    let handlePatientChange = (e) => {
        setPatientId(e.target.value)
    }

    const handleSubmit = async (event) => {
        event.preventDefault();
        //const token = await authService.getAccessToken();
        const response = await fetch('patient',
            {
                method: 'POST',
                body: JSON.stringify({ Date, Notes, PatientId }),
                headers: { 'Content-Type': 'application/json' }
            }
        );
        props.history.push("/visits");
    }

    return (
        <div className="container">
            <h1>Create Visit Entry</h1>
            <div className="row">
                <div className="col-mid-6">
                    <form id="statsForm" onSubmit={handleSubmit}>
                        <div className="form-group">
                            <label htmlFor="visitDate">Visit Date</label>
                            <input type="date" name="visitDate" className="form-control" onChange={(e) => setDate(e.target.value)} />
                        </div>
                        <div className="form-group">
                            <label htmlFor="notes">Notes</label>
                            <input type="text" name="notes" className="form-control" onChange={(e) => setNotes(e.target.value)} />
                        </div>
                        <select onChange={handlePatientChange}>
                            <option value="⬇️ Select a patient ⬇️"> -- Select a patient -- </option>
                            {Patients.map((patient) => <option value={patient.patientId}>{patient.patientName} | {patient.patientId}</option>)}
                        </select>
                        <div className="form-group">
                            <button type="submit" className="btn btn-success">Submit</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default Patients;