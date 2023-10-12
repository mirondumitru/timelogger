import React, { useEffect, useState } from "react";
import { Column } from "react-table";
import { IProject } from "../types/IProject";
import TimeLoggerTable from "../components/Table";
import moment from "moment";
import { get } from "../api/timeRegistrationsApi";
import { get as getProject } from "../api/projectsApi";
import { useSearchParams } from "react-router-dom";
import RedirectButton from "../components/RedirectButton";
import { ITimeRegistration } from "../types/ITimeRegistration";

function pad(number: number) {
    if (number < 10) {
        return "0" + number;
    }
    else {
        return number;
    }
}

const columns: Array<Column> = [
    {
        Header: "Id",
        accessor: "id",
        width: 25,
        minWidth: 25,
    },
    {
        Header: "Project Name",
        accessor: "project.name"
    },
    {
        Header: "Date",
        accessor: "valueDate",
        width: 75,
        minWidth: 75,
        Cell: props => (
            <span>{moment(props.row.values.valueDate).format('YYYY-MM-DD')}</span>
        )
    },
    {
        Header: "Time (hh:mm)",
        accessor: "minutes",
        width: 75,
        minWidth: 75,
        Cell: props => (
            <span>{pad(Math.trunc(props.row.values.minutes / 60))}:{pad(props.row.values.minutes % 60)}</span>
        )
    },
    {
        Header: "Added on",
        accessor: "createdAtUtc",
        width: 75,
        minWidth: 75,
        Cell: props => (
            <span>{moment(props.row.values.createdAtUtc).format('YYYY-MM-DD')}</span>
        )
    },
];

function composeAddUrl(id: string) {
    return "add?projectId=" + id;
}

export default function TimeRegistrations() {
    const [data, setData] = useState<ITimeRegistration[]>([]);
    const [totalTime, setTotalTime] = useState<string>();
    const [projectData, setProjectData] = useState<IProject>();
    const [searchParams] = useSearchParams()

    var projectId = searchParams.get('projectId') || '';

    const fetchData = async () => {
        const response = await get({ projectId: projectId });
        setData(response);

        var total = response.reduce((sum:any, current:any) => sum + current.minutes, 0);
        setTotalTime(pad(Math.trunc(total / 60)) + ':' + pad(total % 60));
    }

    const fetchProjectData = async () => {
        if (projectId) {
            const response = await getProject(projectId);
            setProjectData(response);
        }
    }

    useEffect(() => {
        fetchData();
        fetchProjectData();
    }, []);

    return (
        <>
            <div className="flex items-center my-6"> 
                <div className="w-1/2">
                    <RedirectButton text="Add" path={composeAddUrl(projectId)} disabled={projectData ? projectData.isCompleted : true}></RedirectButton>
                </div>

                <div className="w-1/2 flex justify-end">
                    Total Time Spent : <span className="px-2" style={{ fontWeight: "bold" }}>{totalTime}</span>
                </div>
            </div>

            <TimeLoggerTable columns={columns} data={data} />
        </>
    );
}
