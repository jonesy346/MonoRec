import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

function Doctors(props) {
    //static displayName = Doctors.name;

    const [DoctorId, setDoctorId] = useState(0);
    const [Doctors, setDoctors] = useState([]);

    useEffect(() => {
        populateDoctors();
    }, [Doctors]);

    const incrementDoctorId = () => {
        setDoctorId(DoctorId + 1);
    }

    const populateDoctors = async () => {
        console.log("Populate Doctors function loaded");
        const response = await fetch('doctor', { method: 'GET' })
        const data = await response.json();
        setDoctors(data);
    }

    const tableRows = Doctors.map((val, key) => {
        return (
            <tr key={key}>
                <td>{val.DoctorId}</td>
                <td>{val.DoctorName}</td>
                <td></td>
            </tr>
        );
    });
    
    return (
        <div className="Doctors">
            <h1>Doctors</h1>

            <p>This is a simple example of a React component.</p>

            <p aria-live="polite">Current doctor: <strong>{DoctorId}</strong></p>

            <button className="btn btn-primary" onClick={incrementDoctorId}>Increment</button>

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
        </div>
    );
    
}

export default Doctors;