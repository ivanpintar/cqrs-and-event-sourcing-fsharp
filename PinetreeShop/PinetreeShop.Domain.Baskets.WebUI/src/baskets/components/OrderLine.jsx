import React from 'react';
import { Button } from 'react-bootstrap';

const OrderLine = (line, onChangeQuantity) => (
    <tr key={line.productId}>
        <td>{line.productName}</td>
        <td>{line.price}</td>
        <td>{line.quantity}</td>
        <td>
            <Button onClick={onChangeQuantity}>Change</Button>
        </td>
    </tr>
);

export default OrderLine;