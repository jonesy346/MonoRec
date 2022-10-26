import React, { useState, useEffect } from 'react';
import authService from './api-authorization/AuthorizeService'

function Visits(props) {
    //static displayName = Visits.name;

    const [VisitId, setVisitId] = useState(0);
    const [Visits, setVisits] = useState([]);

    useEffect(() => {
        populateVisits();
    }, [Visits]);

    const incrementVisitId = () => {
        setVisitId(VisitId + 1);
        console.log(Visits);
    }

    const populateVisits = async () => {
        console.log("Populate Visits function loaded");
        const response = await fetch('visit', { method: 'GET' });
        const data = await response.json();
        setVisits(data);
        console.log(data);
    }
    
    return (
        <div className="Visits">
            <h1>Visits</h1>

            <p>This is a simple example of a React component.</p>

            <p aria-live="polite">Current visit: <strong>{VisitId}</strong></p>

            <button className="btn btn-primary" onClick={incrementVisitId}>Increment</button>

            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>PatientId</th>
                        <th>DoctorId</th>
                        <th>Button</th>
                    </tr>
                </thead>
                <tbody>
                    {Visits.map((val, key) => {
                        return (
                            <tr key={key}>
                                <td>{val.visitId}</td>
                                <td>{val.patientId}</td>
                                <td>{val.doctorId}</td>
                                <td></td>
                            </tr>
                        )
                    })}
		        </tbody>
            </table>
        </div>
    );
}

export default Visits;