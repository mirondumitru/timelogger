import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { IProject } from "../types/IProject";
import { get } from "../api/projectsApi";
import { ITimeRegistration } from "../types/ITimeRegistration";
import { save } from "../api/timeRegistrationsApi";

export default function TimeRegistrations() {

    const [searchParams] = useSearchParams()
    const [projectData, setProjectData] = useState<IProject | undefined>();
    const [errors, setErrors] = useState<any[]>([]);
    const [timeRegistrationData, setTimeRegistrationData] = useState<ITimeRegistration>();

    var projectId = searchParams.get('projectId') || '';

    const fetchProjectData = async () => {
        if (projectId) {
            const response = await get(projectId);
            setProjectData(response);
        }
    }

    const navigate = useNavigate();

    const saveTimeRegistration = async () => {
        if (timeRegistrationData) {
            timeRegistrationData.project = projectData;

            console.log(timeRegistrationData);
            var result = await save(timeRegistrationData);
            if (result.errors) {
                setErrors(result.errors);
                console.log(result.errors);
            }
            else {
                navigate("../../timeRegistrations?projectId=" + projectId);
            }
        }
    }

    useEffect(() => {
        fetchProjectData();
        setTimeRegistrationData({ id: 0, minutes: 0, project: projectData, valueDate: new Date() });
    }, []);

    const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        saveTimeRegistration();
    }

    function onTimeChange(e: any): void {
        var splitResult = e.target.value.split(":");

        if (splitResult.length === 2 && timeRegistrationData) {
            timeRegistrationData.minutes = parseInt(splitResult[0]) * 60 + parseInt(splitResult[1]);
        }
    }

    function onDateChange(e: any): void {
        if (timeRegistrationData)
            timeRegistrationData.valueDate = e.currentTarget.value;
    }

    return (
        <><div className="py-2 px-4 disabled w-1/2 my-2 font-bold" style={{ color: 'red' }} hidden={errors?.length == 0}>
            {
                errors.map((e: any) => {
                    return (<span>{e.message}</span>)
                })}
        </div >

            <form onSubmit={(e) => handleSubmit(e)}>
                <input
                    className="border py-2 px-4 disabled w-1/2 my-2"
                    disabled
                    value={projectData?.name}
                    type="text" />
                <br />
                <input
                    className="border py-2 px-4 disabled w-1/2 my-2"
                    placeholder="Time (hh:mm)"
                    onChange={onTimeChange}
                    type="text" />
                <br />
                <input
                    className="border py-2 px-4 disabled w-1/2 my-2"
                    placeholder="Value Date (yyyy-dd-mm)"
                    onChange={onDateChange}
                    type="text" />
                <br />
                <button className="text-white font-bold py-2 px-4 rounded bg-blue-500 hover:bg-blue-700" type="submit">
                    Save
                </button>

            </form></>
    );
}